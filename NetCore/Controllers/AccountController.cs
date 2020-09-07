using System;
using Bcrypt = BCrypt.Net.BCrypt;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetCore.Context;
using NetCore.Model;
using NetCore.Services;
using NetCore.ViewModel;

namespace NetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly MyContext _context;
        private readonly UserManager<User> _userManager;
        AttrEmail attrEmail = new AttrEmail();
        RandomDigit randDig = new RandomDigit();
        SmtpClient client = new SmtpClient();
        public AccountController(MyContext myContext, UserManager<User> userManager)
        {
            _context = myContext;
            _userManager = userManager;

        }
        [HttpPost]
        [Route("code")]
        public ActionResult VerifyCode(UserVm userVm)
        {
            if (ModelState.IsValid)
            {
                var getUserRole = _context.userRoles.Include("user").Include("role").SingleOrDefault(x => x.user.Email == userVm.Email);

                if (getUserRole == null)
                {
                    return NotFound();
                }
                else if (userVm.VerifyCode != getUserRole.user.SecurityStamp)
                {
                    return BadRequest(new { msg = "Your Code is Wrong" });
                }
                else
                {
                    return StatusCode(200, new
                    {
                        Id = getUserRole.user.Id,
                        Username = getUserRole.user.UserName,
                        Email = getUserRole.user.Email,
                        RoleName = getUserRole.role.Name,
                    });
                }
            }
            return BadRequest(500);
        }
        [HttpPost]
        [Route("login")]
        public ActionResult Login(UserVm userVm)
        {
            var isExist = _context.users.Where(Q => Q.Email == userVm.Email || Q.UserName == userVm.Email).FirstOrDefault();
            if (isExist == null)
            {
                return BadRequest("User undefined");
            }
            else if (userVm.Password == null | userVm.Password.Equals(""))
            {
                return BadRequest("Password must be filled");
            }
            else if (!BCrypt.Net.BCrypt.Verify(userVm.Password, isExist.PasswordHash))
            {
                return BadRequest("wrong password");
            }
            else
            {
                var isValid = _context.userRoles.Where(A => A.UserId == isExist.Id).FirstOrDefault();
                var isValidRole = _context.roles.Where(B => B.Id == isValid.RoleId).FirstOrDefault();
                UserVm userVM = new UserVm()
                {
                    Email = isExist.Email,
                    Id = isExist.Id,
                    Password = isExist.PasswordHash,
                    Username = isExist.UserName,
                    RoleName = isValidRole.Name
                };
                return Ok(userVM);
            }


        }



        [HttpPost]
        [Route("register")]
        public IActionResult Register(UserVm userVm)
        {

            if (ModelState.IsValid)
            {
                client.Port = 587;
                client.Host = "smtp.gmail.com";
                client.EnableSsl = true;
                client.Timeout = 10000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(attrEmail.mail, attrEmail.pass);

                var code = randDig.GenerateRandom();
                var fill = "Hi " + userVm.Username + "\n\n"
                          + "Try this Password to get into login: \n"
                          + code
                          + "\n\nThank You";

                MailMessage mm = new MailMessage("donotreply@domain.com", userVm.Email, "Create Email", fill);
                mm.BodyEncoding = UTF8Encoding.UTF8;
                mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                client.Send(mm);
                var uId = new Guid();
                var user = new User
                
                {
                    //Id = uId.ToString(),
                    UserName = userVm.Username,
                    Email = userVm.Email,
                    SecurityStamp = code,
                    PasswordHash = Bcrypt.HashPassword(userVm.Password),
                    PhoneNumber = userVm.Phone,
                    EmailConfirmed = false,
                    PhoneNumberConfirmed = false,
                    TwoFactorEnabled = false,
                    LockoutEnabled = false,
                    AccessFailedCount = 0
                };
                _context.users.Add(user);
                var uRole = new UserRole
                {
                    UserId = user.Id,
                    RoleId = "2"
                };
                _context.userRoles.Add(uRole);
                _context.SaveChanges();
                return Ok("Successfully Created");
            }
            return BadRequest("Not Successfully");
        }
        [HttpGet]
        public List<UserVm> GetAll()
        {
            List<UserVm> list = new List<UserVm>();
            foreach (var item in _context.userRoles)
            {
                UserVm role = new UserVm()
                {
                    Id = item.Id,
                    Username = item.RoleId

                };
                list.Add(role);
            }
            return list;
        }
        [HttpPut("{id}")]
        public IActionResult Update(string id, UserVm userVm)
        {
            var role = _context.roles.Where(r => r.Name == userVm.Username).FirstOrDefault();
            var getId = _context.users.Find(id);
            getId.UserName = userVm.Username;
            getId.Email = userVm.Email;
            var isValid = BCrypt.Net.BCrypt.Verify(userVm.Password, getId.PasswordHash);
            if (isValid) { Ok("Failed Update"); }
            else
            {
                var hashPw = BCrypt.Net.BCrypt.HashPassword(userVm.Password);
                getId.PasswordHash = hashPw;
            }
            getId.PhoneNumber = userVm.Phone;
            var oldRU = _context.userRoles.Where(ru => ru.UserId == id).FirstOrDefault();
            oldRU.role = role;
            _context.SaveChanges();
            return Ok("Successfully Update");
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            var getId = _context.users.Find(id);
            //var getIdUr = _context.userRoles.Where(uid => uid.UserId == getId.Id).FirstOrDefault();
            //var getUserId = _context.userRoles.Find(getIdUr);
            _context.users.Remove(getId);
            //_context.userRoles.Remove(getUserId);
            _context.SaveChanges();
            return Ok("Successfully Delete");
        }


    }
}
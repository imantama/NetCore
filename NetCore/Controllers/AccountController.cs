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
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;

namespace NetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        public IConfiguration _configuration;
        private readonly MyContext _context;
        private readonly UserManager<User> _userManager;
        AttrEmail attrEmail = new AttrEmail();
        RandomDigit randDig = new RandomDigit();
        SmtpClient client = new SmtpClient();
        public AccountController(MyContext myContext, UserManager<User> userManager, IConfiguration config)
        {
            _context = myContext;
            _userManager = userManager;
            _configuration = config;

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
            var getUserRole = _context.userRoles.Include("user").Include("role").SingleOrDefault(x => x.user.Email == userVm.Email);

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
                //var isValid = _context.userRoles.Where(A => A.UserId == isExist.Id).FirstOrDefault();
                //var isValidRole = _context.roles.Where(B => B.Id == isValid.RoleId).FirstOrDefault();
                //UserVm userVM = new UserVm()
                //{
                //    Email = isExist.Email,
                //    Id = isExist.Id,
                //    Password = isExist.PasswordHash,
                //    Username = isExist.UserName,
                //    RoleName = isValidRole.Name,
                if (isExist != null)
                {
                    if (getUserRole.user.SecurityStamp != null)
                    {
                        var claims = new List<Claim> {

                        new Claim("Id", isExist.Id.ToString()),
                        new Claim("UserName", isExist.UserName),
                        new Claim("Email", isExist.Email),
                        new Claim("VerifyCode", getUserRole.user.SecurityStamp)
                        };
                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

                        var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                        var token = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"], claims, expires: DateTime.UtcNow.AddDays(1), signingCredentials: signIn);

                        return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                    }
                    else
                    {
                        var claims = new List<Claim> {
                                new Claim("Id", getUserRole.user.Id),
                                new Claim("Username", getUserRole.user.UserName),
                                new Claim("Email", getUserRole.user.Email),
                                new Claim("RoleName", getUserRole.role.Name)
                            };
                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                        var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                        var token = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"], claims, expires: DateTime.UtcNow.AddDays(1), signingCredentials: signIn);
                        return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                    }
                   
                }
                return BadRequest("Invalid credentials");
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
                          + "Use this verification code to login: \n"
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
                    RoleId = "1"
                };
                _context.userRoles.Add(uRole);
                var emp = new Employee
                {
                    EmployeeId = user.Id,
                    CreateDate = DateTimeOffset.Now,
                    isDelete = false
                };
                _context.employees.Add(emp);
                _context.SaveChanges();
                return Ok("Successfully Created");
            }
            return BadRequest("Not Successfully");
        }

        [Route("logout")]
        public IActionResult Logout()
        {
            //var jwtauthmanager = new IJwtAuthManager();
            //var userName = User.Identity.Name;
            //_jwtAuthManager.RemoveRefreshTokenByUserName(userName); // can be more specific to ip, user agent, device name, etc.
            //_logger.LogInformation($"User [{userName}] logged out the system.");

            //HttpContext.Session.Remove("lvl");
            HttpContext.Session.Clear();
            return Redirect("/login");
        }

        [Authorize]
        [HttpGet]
        public List<UserVm> GetAll()
        {
            List<UserVm> list = new List<UserVm>();
            
            foreach (var item in _context.users)
            {
                //var role = _context.userRoles.Where(r => r.UserId ==  ).FirstOrDefault();
                UserVm user = new UserVm()
                {
                    Id = item.Id,
                    Username = item.UserName,
                    Email = item.Email, 
                    Phone = item.PhoneNumber
                    

                };
                list.Add(user);
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

        private string GetJWT(UserVm userVm)
        {
            var claims = new List<Claim> {
                            new Claim("Id", userVm.Id),
                            new Claim("Username", userVm.Username),
                            new Claim("Email", userVm.Email),
                            new Claim("RoleName", userVm.RoleName),
                            new Claim("VerifyCode", userVm.VerifyCode == null ? "" : userVm.VerifyCode),
                        };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                            _configuration["Jwt:Issuer"],
                            _configuration["Jwt:Audience"],
                            claims,
                            expires: DateTime.UtcNow.AddDays(1),
                            signingCredentials: signIn
                        );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }


    }
}
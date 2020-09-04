using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetCore.Context;
using NetCore.Model;
using NetCore.ViewModel;

namespace NetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly MyContext _context;
        public AccountController(MyContext myContext)
        {
            _context = myContext;
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
            else {
                var isValid = _context.userRoles.Where(A => A.UserId == isExist.Id).FirstOrDefault();
                var isValidRole = _context.roles.Where(B => B.Id == isValid.RoleId).FirstOrDefault();
                UserVm userVM = new UserVm() {
                    Email = isExist.Email,
                    Id= isExist.Id,
                    Password= isExist.PasswordHash,
                    Username= isExist.UserName,
                    RoleName= isValidRole.Name
                };
                return Ok(userVM);
            }
            

        }
        //[HttpPost]
        ////[HttpGet("{}")]
        //[Route("Login")]
        //public IActionResult Login(UserVm userVm)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var getUserRole = _context.userRoles.Include("User").Include("Role").SingleOrDefault(x => x.user.Email == userVm.Email);
        //        if (getUserRole == null)
        //        {
        //            return NotFound();
        //        }
        //        else if (userVm.Password == null || userVm.Password.Equals(""))
        //        {
        //            return BadRequest(new { msg = "Password must filled" });
        //        }
        //        else if (!BCrypt.Net.BCrypt.Verify(userVm.Password, getUserRole.user.PasswordHash))
        //        {
        //            return BadRequest(new { msg = "Password is Wrong" });
        //        }
        //        else
        //        {
        //            var user = new UserVm();
        //            user.Id = getUserRole.user.Id;
        //            user.Username = getUserRole.user.UserName;
        //            user.Email = getUserRole.user.Email;
        //            user.Password = getUserRole.user.PasswordHash;
        //            user.Phone = getUserRole.user.PhoneNumber;
        //            user.RoleName = getUserRole.role.Name;
        //            return StatusCode(200, user);
        //        }
        //    }
        //    return BadRequest(500);
        //}


        [HttpPost]
        [Route("register")]
        public IActionResult Register(UserVm userVm)
        {

            string hashPw = BCrypt.Net.BCrypt.HashPassword(userVm.Password);
            var Id = Guid.NewGuid().ToString();
            var user = new User

            {
                Id = Id,
                UserName = userVm.Username,
                Email = userVm.Email,
                EmailConfirmed = false,
                PasswordHash = hashPw,
                PhoneNumber = userVm.Phone,
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnabled = false,
                AccessFailedCount = 0

                //return data;
            };
            _context.users.AddAsync(user);
            var role = new UserRole
            {
                Id = Guid.NewGuid().ToString(),
                UserId = Id,
                RoleId = "e61b749c-51b5-4b2b-99bb-e7b9b31edeb1"

            };
            _context.userRoles.AddAsync(role);
            _context.SaveChanges();
            return Ok("Successfully Created");
        }
        [HttpGet]
        public List<UserVm> GetAll()
        {
            List<UserVm> list = new List<UserVm>();
            foreach (var item in _context.userRoles)
            {
                UserVm role = new UserVm()
                {
                    Id=item.Id,
                    Username=item.RoleId
                    
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
            var getIdUr = _context.userRoles.Where(ur => ur.UserId == id).FirstOrDefault();
            _context.users.Remove(getId);
            _context.userRoles.Remove(getIdUr);
            _context.SaveChanges();
            return Ok("Successfully Delete");
        }
    }

}
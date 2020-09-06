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
    
    public class UsersController : ControllerBase
    {
       
        private readonly MyContext _context;
        public UsersController(MyContext myContext) {
            _context = myContext;
        }
        [HttpGet]
        //public async Task<List<User>> GetAll()
        public List<UserVm> GetAll()
        {
            List<UserVm> list = new List<UserVm>();
            foreach (var item in _context.users)
            {
                UserVm user = new UserVm()
                {
                    Id = item.Id,
                    Username = item.UserName,
                    Email = item.Email,
                    Password = item.PasswordHash,
                    Phone = item.PhoneNumber
                };
                list.Add(user);
            }
            return list;
        }
        [HttpGet("{id}")]
        public UserVm GetID(string id)
        {
            var getId = _context.users.Find(id);
            UserVm user = new UserVm()
            {
                Id = getId.Id,
                Username = getId.UserName,
                Email = getId.Email,
                Password = getId.PasswordHash,
                Phone = getId.PhoneNumber
            };
            return user;
        }
        [HttpPost]
        public async Task<IActionResult> Create(UserVm userVm)
        {
            string hashPw = BCrypt.Net.BCrypt.HashPassword(userVm.Password);
            var user = new User();
            user.Id = userVm.Id;
            user.UserName = userVm.Username;
            user.Email = userVm.Email;
            user.EmailConfirmed = false;
            user.PasswordHash = hashPw;
            user.PhoneNumber = userVm.Phone;
            user.PhoneNumberConfirmed = false;
            user.TwoFactorEnabled = false;
            user.LockoutEnabled = false;
            user.AccessFailedCount = 0;
            var data = await _context.users.AddAsync(user);
            _context.SaveChanges();
            return Ok("Successfully Created");
            //return data;
        }
        [HttpPut("{id}")]
        public IActionResult Update(string id, UserVm userVm)
        {
            var getId = _context.users.Find(id);
            getId.UserName = userVm.Username;
            getId.Email = userVm.Email;
            getId.PasswordHash = userVm.Password;
            getId.PhoneNumber = userVm.Phone;
            var data = _context.users.Update(getId);
            _context.SaveChanges();
            if (data.IsKeySet == true)

            {
               
                return Ok("Update Succesfully");
               
            }
            return BadRequest("Not Successfully");
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            var getId = _context.users.Find(id);
            var data = _context.users.Remove(getId);
            _context.SaveChanges();
            return Ok("Pas Mantap");
        }
    }
}
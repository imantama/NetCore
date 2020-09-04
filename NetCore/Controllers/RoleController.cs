using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetCore.Context;
using NetCore.Model;
using NetCore.ViewModel;

namespace NetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly MyContext _myContext;
        public RoleController(MyContext myContext) {
            _myContext = myContext;
        }
        [HttpGet]
        //public async Task<List<User>> GetAll()
        public List<UserVm> GetAll()
        {
            List<UserVm> list = new List<UserVm>();
            foreach (var item in _myContext.roles)
            {
                UserVm user = new UserVm()
                {
                    Id = item.Id,
                    Username = item.Name
                    
                };
                list.Add(user);
            }
            return list;
            //return Ok("mantap");
        }
        [HttpGet("{id}")]
        public UserVm GetID(string id)
        {
            var getId = _myContext.users.Find(id);
            UserVm user = new UserVm()
            {
                Id = getId.Id,
                Username = getId.UserName
                
            };
            return user;
        }
        [HttpPost]
        public async Task<IActionResult> Create(Role role)
        {
            
            var user = new Role();
            user.Id = Guid.NewGuid().ToString();
            user.Name = role.Name;
            var data = await _myContext.roles.AddAsync(user);
            _myContext.SaveChanges();
            return Ok("Successfully Created");
            //return data;
        }
        [HttpPut("{id}")]
        public IActionResult Update(string id, UserVm userVmRole)
        {
            var getId = _myContext.roles.Find(id);
            getId.Name = userVmRole.Username;
            
            var data = _myContext.roles.Update(getId);
            _myContext.SaveChanges();
            if (data.IsKeySet == true)

            {

                return Ok("Update Succesfully");

            }
            return BadRequest("Not Successfully");
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            var getId = _myContext.roles.Find(id);
            var data =_myContext.roles.Remove(getId);
            _myContext.SaveChanges();
            return Ok("Pas Mantap");
        }
    }
}
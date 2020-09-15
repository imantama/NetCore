using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NetCore.Context;
using NetCore.ViewModel;

namespace NetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        //public IConfiguration _configuration;
        private readonly MyContext _context;
        public EmployeesController(MyContext myContext/*, IConfiguration iconfiguration*/)
        {
            _context = myContext;
            //_configuration = iconfiguration;
        }
        [HttpGet]
        public async Task<List<EmployeeVm>> GetAll()
        {
            var getData = await _context.employees.Include("user").Where(x => x.isDelete == false).ToListAsync();
            //var getData = await _context.employees.Where(x => x.isDelete==false).ToListAsync();
            List<EmployeeVm> list = new List<EmployeeVm>();
            foreach (var employee in getData)
            {
                EmployeeVm emp = new EmployeeVm()
                {
                    EmployeeId = employee.user.Id,
                    EmployeeName = employee.user.UserName,
                    Address = employee.Address,
                    Phone = employee.user.PhoneNumber,
                    CreateDate = employee.CreateDate,
                    UpdateDate = employee.UpdateDate,
                    DeleteDate = employee.DeleteDate
                };
                list.Add(emp);
            }
            return list;
        }

        [HttpGet("{id}")]
        public EmployeeVm GetID(string id)
        {
            var getData = _context.employees.Include("user").SingleOrDefault(x => x.EmployeeId == id);
            EmployeeVm emp = new EmployeeVm()
            {
                EmployeeId = getData.EmployeeId,
                EmployeeName = getData.user.UserName,
                Address = getData.Address,
                Phone = getData.user.PhoneNumber,
                CreateDate = getData.CreateDate,
                UpdateDate = getData.UpdateDate,
                DeleteDate = getData.DeleteDate
            };
            return emp;
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            if (id != null)
            {
                var getData = _context.employees.Include("user").SingleOrDefault(x => x.EmployeeId == id);
                if (getData == null)
                {
                    return BadRequest("Not Seccessfully");
                }

                getData.DeleteDate = DateTimeOffset.Now;
                getData.isDelete = true;


                _context.Entry(getData).State = EntityState.Modified;
                _context.SaveChanges();
                return Ok("Successfully Delete");
            }
            return Ok("Delete Failed");

        }
    }
}
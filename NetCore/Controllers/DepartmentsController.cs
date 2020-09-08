using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetCore.Base;
using NetCore.Model;
using NetCore.Repository;

namespace NetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : BaseController<Department, DepartmentRepo>
    {
        private readonly DepartmentRepo _department;
        public DepartmentsController (DepartmentRepo departmentRepo): base(departmentRepo)
            {
            _department = departmentRepo;
            }
        [HttpPut("{id}")]
        public async Task<ActionResult<int>> Update(int id, Department department)
        {
            var findId = await _department.GetById(id);
            findId.Name = department.Name;
            var updateDep = await _department.Update(findId);
            if (updateDep.Equals(null))
            {
                return BadRequest("Failed");
            }
            return Ok("Update Successfull");
        }
    }
    
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NetCore.Base;
using NetCore.Model;
using NetCore.Repository;

namespace NetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DivisionsController : BaseController<Division, DivisionRepo>
    {

        private readonly DivisionRepo _division;
        public DivisionsController(DivisionRepo divisionRepo) : base(divisionRepo)
        {
            _division = divisionRepo;
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<int>> Update(int id, Division division)
        {
            var findId = await _division.GetById(id);
            findId.Name = division.Name;
            findId.DepartmentId = division.DepartmentId;
            var updateDep = await _division.Update(findId);
            if (updateDep.Equals(null))
            {
                return BadRequest("Failed");
            }
            return Ok("Update Successfull");
        }
    }
        
}
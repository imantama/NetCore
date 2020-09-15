using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetCore.Repository.Interface;

namespace NetCore.Base
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController<TEntity, TRepository> : ControllerBase
        where TEntity : class
        where TRepository : IRepositories<TEntity>
    {
        private IRepositories<TEntity> _repo;
        public BaseController(TRepository repository) {
            this._repo = repository;
        }
        [HttpGet]

        public async Task<IEnumerable<TEntity>> GetAll() => await _repo.GetAll();

        [HttpGet("{id}")]
        public async Task<ActionResult<TEntity>> GetById(int id) => await _repo.GetById(id);

        [HttpPost]
        public async Task<ActionResult<TEntity>> Post(TEntity entity)
        {
            var data = await _repo.Create(entity);
            if (data>0)
            {
                return Ok("mantap");
            }
            return BadRequest("Create UnSuccesfull");
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<int>> Delete(int id)
        {
            var deleted = await _repo.Delete(id);
            if (!(deleted>0))
            {
                return NotFound("Data is not found");
            }
            return Ok();
        }

        [HttpPut]
        public async Task<ActionResult<int>> Update(TEntity entity)
        {
            var data = await _repo.Update(entity);
            if (data.Equals(null))
            {
                return BadRequest("Something Wrong Please check again");
            }
            return data;
        }
    }

}
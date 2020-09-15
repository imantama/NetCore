using Microsoft.EntityFrameworkCore;
using NetCore.Base;
using NetCore.Context;
using NetCore.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCore.Repository
{
    public class GeneralRepository<TEntity, TContext> : IRepositories<TEntity>
        where TEntity : class, BaseModels
        where TContext :MyContext
    {
        private MyContext _context;
        public GeneralRepository(MyContext myContext)
        {
            _context = myContext;
        }

        public async Task<int> Create(TEntity entity)
        {
            entity.CreateDate = DateTimeOffset.Now;
            entity.isDelete = false;
            await _context.Set<TEntity>().AddAsync(entity);
            var createItem = await _context.SaveChangesAsync();
            if (createItem > 0)
            {
                return createItem;
            }
            return 0;
        }

        public async Task<int> Delete(int id)
        {
            var delete = await GetById(id);
            if (delete == null)
            {
                return 0;
            }
            
            delete.DeleteDate = DateTimeOffset.Now;
            delete.isDelete = true;
            _context.Entry(delete).State = EntityState.Modified;
            return await _context.SaveChangesAsync();
        }

        public virtual async Task<List<TEntity>> GetAll()
        {
            var getAll = await _context.Set<TEntity>().Where(i => i.isDelete == false).ToListAsync();
            if (!getAll.Count().Equals(0))
            {
                return getAll;
            }
            else
            {
                return null;
            }
            
        }
        


        public async Task<TEntity> GetById(int id)
        {
            var getById = await _context.Set<TEntity>().FindAsync(id);
            if (getById != null)
            {
                return getById;
            }
            return null;
        }

        public async Task<int> Update(TEntity entity)
        {
            entity.UpdateDate = DateTimeOffset.Now;
            entity.isDelete = false;
            _context.Entry(entity).State = EntityState.Modified;
            return await _context.SaveChangesAsync(); 
        }
    }
}

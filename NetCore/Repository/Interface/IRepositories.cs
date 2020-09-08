using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCore.Repository.Interface
{
    public interface IRepositories<T> where T : class
    {
        Task<List<T>> GetAll();
        Task<T> GetById(int id);
        Task<int> Update(T entity);
        Task<int> Create(T entity);
        Task<int> Delete(int id);
    }
}

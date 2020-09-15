using Microsoft.EntityFrameworkCore;
using NetCore.Context;
using NetCore.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCore.Repository
{
    public class DivisionRepo : GeneralRepository<Division, MyContext>
    {
        private MyContext _context;
        public DivisionRepo(MyContext context) : base(context) {
            _context = context;
        }

        public override async Task<List<Division>> GetAll()
        {
            var divData = await _context.divisions.Include("Department").Where(x => x.isDelete == false).ToListAsync();
            return divData;
        }
    }
    
}

using NetCore.Context;
using NetCore.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCore.Repository
{
    public class DepartmentRepo:GeneralRepository<Department, MyContext>
    {
        public DepartmentRepo(MyContext context) : base(context) {  }
    }
}

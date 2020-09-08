using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCore.Base
{
    public interface BaseModels
    {
        int Id { get; set; }
        string Name { get; set; }
        DateTimeOffset CreateDate { get; set; }
        DateTimeOffset DeleteDate { get; set; }
        DateTimeOffset UpdateDate { get; set; }
        bool isDelete { get; set; }
    }

}

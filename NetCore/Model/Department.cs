using NetCore.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NetCore.Model
{
    [Table("Departments")]
    public class Department:BaseModels
    {
        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public DateTimeOffset CreateDate { get; set; }
        
        public DateTimeOffset DeleteDate { get; set; }
        
        public DateTimeOffset UpdateDate { get; set; }
        
        public bool isDelete { get; set; }
        
    }
}

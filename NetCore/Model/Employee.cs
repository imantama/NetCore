using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NetCore.Model
{
    [Table("Employee")]
    public class Employee
    {

        [ForeignKey("User")]
        public string EmployeeId { get; set; }

        public string EmployeeName { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

        public DateTimeOffset CreateDate { get; set; }

        public DateTimeOffset DeleteDate { get; set; }

        public DateTimeOffset UpdateDate { get; set; }

        public bool isDelete { get; set; }

        public virtual User user{get; set;}
        
    }
}

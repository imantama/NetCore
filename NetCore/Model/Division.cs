using NetCore.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NetCore.Model
{
    [Table("Division")]
    public class Division : BaseModels
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public int DepartmentId {get; set;}

        public DateTimeOffset CreateDate { get; set; }

        public DateTimeOffset DeleteDate { get; set; }

        public DateTimeOffset UpdateDate { get; set; }

        public bool isDelete { get; set; }

        [ForeignKey("DepartmentId")]
        public virtual Department Department { get; set; }

    }
}

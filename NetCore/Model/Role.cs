using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NetCore.Model
{
    [Table("Tb_Role")]
    public class Role:IdentityRole
    {
        public string Id { get; set; }
        public ICollection<UserRole> userRoles { get; set; }
        public string Name { get; set; }
    }
}

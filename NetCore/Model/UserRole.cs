using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NetCore.Model
{
    [Table("Tb_UserRole")]
    public class UserRole : IdentityUserRole<string>
    {
        [Key]
        public string Id { get; set; }
        public User user { get; set; }
        public Role role { get; set; }
    }
}

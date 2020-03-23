using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;

namespace ZaporArrowAPI.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public bool IsAdmin { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using doft.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace doft.Domain.Enums
{
    public class AppUserRole : IdentityRole
    {
        public virtual AppUser User { get; set; }  // Navigation Property
    }
}
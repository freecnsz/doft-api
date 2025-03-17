using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using doft.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace doft.Domain.Entities
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; }
        public string ProfilePictureUrl { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation Properties
        public virtual ICollection<AppUserRole> AppUserRoles { get; set; }
        public virtual ICollection<Preference> Preferences { get; set; }
        public virtual ICollection<DoftTask> DoftTasks { get; set; }
        public virtual ICollection<Note> Notes { get; set; }
        public virtual ICollection<Event> Events { get; set; }
        public virtual ICollection<Reminder> Reminders { get; set; }
        public virtual ICollection<Attachment> Attachments { get; set; }

    }
}
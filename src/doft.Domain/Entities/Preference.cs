using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace doft.Domain.Entities
{
    public class Preference
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Language { get; set; }
        public string Theme { get; set; }
        public bool NotificationEnabled { get; set; }
        public bool EmailNotifications { get; set; }
        public bool PushNotifications { get; set; }
        public string TimeZone { get; set; }
        public string DateFormat { get; set; }
        public string TimeFormat { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation Property
        public virtual AppUser User { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using doft.Domain.Enums;

namespace doft.Domain.Entities
{
    public class Reminder
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public string UserId { get; set; }
        public ReminderPeriod ReminderPeriod { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ReminderTime { get; set; }

        // Navigation Properties
        public virtual DoftTask DoftTask { get; set; }
        public virtual Event Event { get; set; }
        public virtual Note Note { get; set; }
        public virtual AppUser User { get; set; }
    }
}
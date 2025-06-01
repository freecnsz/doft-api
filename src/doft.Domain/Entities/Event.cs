using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using doft.Domain.Enums;

namespace doft.Domain.Entities
{
    public class Event
    {
        public int Id { get; set; }
        public string OwnerId { get; set; }
        public int CategoryId { get; set; }
        public int DetailId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Location { get; set; }
        public bool IsWholeDay { get; set; }
        public int RepeatId { get; set; }
        public DoftTaskStatus Status { get; set; }
        public DoftTaskPriority Priority { get; set; }
        public double PriorityScore { get; set; }

        // Navigation Properties        
        public virtual AppUser Owner { get; set; }
        public virtual Category Category { get; set; }
        public virtual Detail Detail { get; set; }
        public virtual ICollection<TagLink> TagLinks { get; set; } = new List<TagLink>();
        public virtual ICollection<Reminder> Reminders { get; set; } = new List<Reminder>();
    }
}
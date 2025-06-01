using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace doft.Domain.Entities
{
    public class PlannedTask
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public string UserId { get; set; }
        public DateTime PlannedDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsCompleted { get; set; }

        // Navigation properties
        public virtual DoftTask DoftTask { get; set; }
    }
}
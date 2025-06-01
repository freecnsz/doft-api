using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace doft.Application.DTOs.Schedule
{
    public class ScheduledTaskResultDto
    {
        public int TaskId { get; set; }
        public DateTime PlannedDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTime DueDate { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using doft.Domain.Enums;

namespace doft.Application.DTOs.Schedule
{
    public class ScheduleTaskRequestDto
    {
        public int TaskId { get; set; }
        public string UserId { get; set; }
        public DateTime DueDate { get; set; }

        public Consequence Consequence { get; set; }
        public DueDateOption DueDateOption { get; set; }
        public Duration Duration { get; set; }
        public Urgency Urgency { get; set; }
    }
}
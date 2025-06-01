using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using doft.Domain.Enums;

namespace doft.Application.DTOs.Reminder
{
    public class GetRemindersByTaskIdResponseDto
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public string TaskTitle { get; set; }
        public string TaskDescription { get; set; }
        public string TaskStatus { get; set; }
        public string TaskPriority { get; set; }
        public DateTime TaskDueDate { get; set; }
        public ReminderPeriod ReminderPeriod { get; set; } 
        public DateTime CreatedAt { get; set; }
        public DateTime ReminderTime { get; set; }
    }
}
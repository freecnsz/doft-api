using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using doft.Application.DTOs.Schedule;
using doft.Domain.Enums;

namespace doft.Application.DTOs.Task
{
    public class AddTaskResponseDto
    {
        public int TaskId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public int? CategoryId { get; set; }
        public DoftTaskStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<string> Tags { get; set; }
        public ScheduledTaskResultDto? SchedulingResult { get; set; }
    }
}
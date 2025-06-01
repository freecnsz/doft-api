using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace doft.Application.DTOs.Schedule
{
    public class SchedulingResultDto
    {
        public List<int> ScheduledTaskIds { get; set; } = new();
        public List<int> UnscheduledTaskIds { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
        public List<string> Errors { get; set; } = new();

        public bool Success { get; set; }
        public string? Message { get; set; }
    }
}
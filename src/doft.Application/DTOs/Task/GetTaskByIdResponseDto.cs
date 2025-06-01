using System;
using System.Collections.Generic;

namespace doft.Application.DTOs.Task
{
    public class GetTaskByIdResponseDto
    {
        public int TaskId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool HasAttachment { get; set; }
        public bool HasTag { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string CategoryName { get; set; }
        public string CategoryColor { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public double PriorityScore { get; set; }
        public int RepeatId { get; set; }
        public List<string> Tags { get; set; }
    }
} 
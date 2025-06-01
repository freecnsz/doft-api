using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace doft.Application.DTOs.Task
{
    public class GetAllForUserResponseDto
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
        public double PriorityScore { get; set; } = 0.0; // Default value, can be set based on business logic
        public int RepeatId { get; set; } = 0; // Default value, can be set based on business logic
        public List<string> Tags { get; set; } = new List<string>(); // Assuming tags are stored as a list of strings
        
    }
}
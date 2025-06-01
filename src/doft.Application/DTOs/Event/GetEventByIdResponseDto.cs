using System;
using System.Collections.Generic;

namespace doft.Application.DTOs.Event
{
    public class GetEventByIdResponseDto
    {
        public int EventId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool HasAttachment { get; set; }
        public bool HasTag { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string CategoryName { get; set; }
        public string CategoryColor { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Location { get; set; }
        public bool IsWholeDay { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public double PriorityScore { get; set; }
        public int RepeatId { get; set; }
        public List<string> Tags { get; set; }
    }
} 
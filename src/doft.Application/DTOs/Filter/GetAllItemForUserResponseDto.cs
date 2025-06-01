using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using doft.Application.DTOs.Detail;

namespace doft.Application.DTOs.Filter
{
    public class GetAllItemForUserResponseDto
    {
        public int ItemId { get; set; }
        public string ItemType { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool HasAttachment { get; set; }
        public bool HasTag { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string CategoryName { get; set; }
        public string CategoryColor { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string Priority { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public double PriorityScore { get; set; } = 0.0;
        public string Location { get; set; } = string.Empty;
        public bool IsWholeDay { get; set; } = false;
        public int RepeatId { get; set; } = -1;
        public List<String> Tags { get; set; } = new List<string>();
    }
}
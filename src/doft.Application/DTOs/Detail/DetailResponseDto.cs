using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace doft.Application.DTOs.Detail
{
    public class DetailResponseDto
    {
        public int ItemId { get; set; }
        public string ItemType { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool HasAttachment { get; set; }
        public bool HasTag { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
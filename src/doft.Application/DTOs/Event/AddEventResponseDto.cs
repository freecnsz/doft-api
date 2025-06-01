using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace doft.Application.DTOs.Event
{
    public class AddEventResponseDto
    {
        public int EventId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
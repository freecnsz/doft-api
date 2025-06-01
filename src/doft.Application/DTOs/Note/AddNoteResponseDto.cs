using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace doft.Application.DTOs.Note
{
    public class AddNoteResponseDto
    {
        public string Id { get; set; }
        public  DateTime CreatedAt { get; set; }
    }
}
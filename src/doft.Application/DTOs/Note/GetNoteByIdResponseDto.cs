using System;
using System.Collections.Generic;

namespace doft.Application.DTOs.Note
{
    public class GetNoteByIdResponseDto
    {
        public int NoteId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public bool HasAttachment { get; set; }
        public bool HasTag { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string CategoryName { get; set; }
        public string CategoryColor { get; set; }
        public List<string> Tags { get; set; }
    }
} 
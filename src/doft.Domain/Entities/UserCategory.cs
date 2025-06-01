using System;

namespace doft.Domain.Entities
{
    public class UserCategory
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int CategoryId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public Category Category { get; set; }
    }
} 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace doft.Domain.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public DateTime CreatedAt { get;  set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public ICollection<UserCategory> UserCategories { get; set; }
        public ICollection<Event> Events { get; set; }
        public ICollection<DoftTask> DoftTasks { get; set; }
        public ICollection<Note> Notes { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace doft.Domain.Entities
{
    public class Note
    {
        public int Id { get; set; }
        public string OwnerId { get; set; }
        public int DetailId { get; set; }
        public int CategoryId { get; set; }
        public string Content { get; set; }

        // Navigation Properties
        public virtual AppUser Owner { get; set; }
        public virtual Detail Detail { get; set; }
        public virtual Category Category { get; set; }
        public virtual ICollection<TagLink> TagLinks { get; set; } = new List<TagLink>();
    }
}
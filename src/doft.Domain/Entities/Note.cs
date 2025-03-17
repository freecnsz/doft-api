using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace doft.Domain.Entities
{
    public class Note
    {
        public int Id { get; set; }
        public string OwnerId { get; set; }
        public int DetailId { get; set; }
        public string Content { get; set; } = string.Empty;

        // Navigation Properties
        public virtual AppUser Owner { get; set; }
        public virtual Detail Detail { get; set; }
    }
}
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

        // Navigation Properties
        public virtual ICollection<DoftTask> DoftTasks { get; set; }
    }
}
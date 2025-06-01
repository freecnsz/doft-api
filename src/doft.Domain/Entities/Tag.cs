using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace doft.Domain.Entities
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Navigation Properties
        public virtual ICollection<TagLink> TagLinks { get; set; }


    }
}
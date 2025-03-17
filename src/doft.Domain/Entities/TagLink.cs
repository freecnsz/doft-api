using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using doft.Domain.Enums;

namespace doft.Domain.Entities
{
    public class TagLink
    {
        public int Id { get; set; }
        public ItemType ItemType { get; set; }
        public int ItemId { get; set; }
        public int TagId { get; set; }

        // Navigation Properties
        public virtual Tag Tag { get; set; }
    }
}
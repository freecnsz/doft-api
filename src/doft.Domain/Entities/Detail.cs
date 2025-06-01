using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using doft.Domain.Enums;

namespace doft.Domain.Entities
{
    public class Detail
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public ItemType ItemType { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool HasAttachment { get; set; }
        public bool HasTag { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties
        public virtual DoftTask DoftTask { get; set; }
        public virtual Event Event { get; set; }
        public virtual Note Note { get; set; }

    }
}
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
    public ItemType ItemType { get; set; } // enum: Note, Event, Task
    public int ItemId { get; set; }
    public int TagId { get; set; }

    // Only real relationship
    public virtual Tag Tag { get; set; }

    // Optional use - not configured in EF
    public virtual Note Note { get; set; }
    public virtual Event Event { get; set; }
    public virtual DoftTask DoftTask { get; set; }
}

}
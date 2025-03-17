using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace doft.Domain.Entities
{
    public class Event
    {
    public int Id { get; set; }
    public string OwnerId { get; set; }
    public int DetailId { get; set; }
    public DateTime EventDate { get; set; }
    public string Location { get; set; }
    public bool IsWholeDay { get; set; }

    // Navigation Properties
    public virtual AppUser Owner { get; set; }
    public virtual Detail Detail { get; set; }
    }
}
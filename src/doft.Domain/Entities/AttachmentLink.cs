using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using doft.Domain.Enums;

namespace doft.Domain.Entities
{
    public class AttachmentLink
    {
    public int Id { get; set; }
    public int AttachmentId { get; set; }
    public ItemType ItemType { get; set; }
    public int ItemId { get; set; }

    // Navigation Properties
    public virtual Attachment Attachment { get; set; }
    }
}
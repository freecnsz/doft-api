using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace doft.Domain.Entities
{
    public class Attachment
    {
        public int Id { get; set; }
        public string OwnerId { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public int FileSize { get; set; }
        public string FilePath { get; set; }
        public DateTime UploadedAt { get; set; }

        // Navigation Properties
        public virtual AppUser Owner { get; set; }
    }
}
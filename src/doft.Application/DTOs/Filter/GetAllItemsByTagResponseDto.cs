using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using doft.Application.DTOs.Detail;
using doft.Domain.Entities;

namespace doft.Application.DTOs.Filter
{
    public class GetAllItemsByTagResponseDto
    {
        public int ItemId { get; set; }
        public string ItemType { get; set; }
        public DetailResponseDto ItemDetail { get; set; }
        
    }
}
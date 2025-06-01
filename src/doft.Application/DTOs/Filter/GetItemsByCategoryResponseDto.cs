using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using doft.Application.DTOs.Detail;

namespace doft.Application.DTOs.Filter
{
    public class GetItemsByCategoryResponseDto
    {
        public int ItemId { get; set; }
        public string ItemType { get; set; }
        public DetailResponseDto Details { get; set; }
    }
}
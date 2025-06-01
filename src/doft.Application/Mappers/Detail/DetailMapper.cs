using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using doft.Application.DTOs.Detail;

namespace doft.Application.Mappers.Detail
{
    public static class DetailMapper
    {
        public static DetailResponseDto FromDetailToDetailResponseDto(this doft.Domain.Entities.Detail detail)
        {
            if (detail == null) return null;

            return new DetailResponseDto
            {
                ItemId = detail.ItemId,
                ItemType = detail.ItemType.ToString(),
                Title = detail.Title,
                Description = detail.Description,
                HasAttachment = detail.HasAttachment,
                HasTag = detail.HasTag,
                CreatedAt = detail.CreatedAt,
                UpdatedAt = detail.UpdatedAt
            };
        }

    }
}
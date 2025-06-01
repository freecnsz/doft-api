using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using doft.Domain.Entities;
using doft.Domain.Enums;

namespace doft.Application.Interfaces.RepositoryInterfaces
{
    public interface ITagLinkRepository : IGenericRepository<TagLink>
    {
        Task<List<TagLink>> GetAllItemsByTagAsync(int tagId);
        Task<List<TagLink>> GetTagLinksByItemAsync(ItemType itemType, int itemId);
    }
}
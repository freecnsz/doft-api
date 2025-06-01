using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using doft.Application.Interfaces.RepositoryInterfaces;
using doft.Domain.Entities;

namespace doft.Application.Interfaces.RepositoryInterfaces
{
    public interface ITagRepository : IGenericRepository<Tag>
    {
        Task<Tag> GetTagByNameAsync(string name);
    }
}
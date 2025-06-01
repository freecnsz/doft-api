using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using doft.Application.Interfaces.RepositoryInterfaces;
using doft.Domain.Entities;

namespace doft.Infrastructure.Repositories
{
    public class DetailRepository : GenericRepository<Detail>, IDetailRepository
    {
       private readonly ApplicationDbContext _context;

        public DetailRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

    }
}
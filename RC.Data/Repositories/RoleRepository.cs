using Microsoft.EntityFrameworkCore;
using RC.Data.Database;
using RC.Domain.Entities;
using RC.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace RC.Data.Repositories
{
    public class RoleRepository(RCDbContext context) : IRoleRepository
    {
        private readonly RCDbContext _context = context;

        public async Task<IEnumerable<Role>> GetAllAsync(int currentPage, int pageSize)
        {
            return await _context.Set<Role>()
                .OrderBy(r => r.Id)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Role?> GetByIdAsync(long id)
        {
            return await _context.Set<Role>()
                .Where(r => r.Id == id)
                .FirstOrDefaultAsync();
        }
    }
}

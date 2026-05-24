using Microsoft.EntityFrameworkCore;
using RC.Data.Database;
using RC.Domain.Entities;
using RC.Domain.Interfaces.Repositories;

namespace RC.Data.Repositories
{
    public class OrganizationRepository(RCDbContext context) : IOrganizationRepository
    {
        private readonly RCDbContext _context = context;

        public async Task<Organization> AddAsync(Organization newOrganization)
        {
            await _context.AddAsync(newOrganization);
            await _context.SaveChangesAsync();

            return newOrganization;
        }

        public async Task<IEnumerable<Organization>> GetAllAsync(int currentPage, int pageSize)
        {
            return await _context.Set<Organization>()
                .OrderBy(o => o.Id)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetAllTotalAsync()
        {
            return await _context.Set<Organization>().CountAsync();
        }

        public async Task<Organization?> GetByIdAsync(long id)
        {
            return await _context.Set<Organization>()
                .Where(o => o.Id == id)
                .FirstOrDefaultAsync();
        }
    }
}

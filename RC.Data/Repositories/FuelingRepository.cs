using Microsoft.EntityFrameworkCore;
using RC.Data.Database;
using RC.Domain.Entities;
using RC.Domain.Interfaces.Repositories;

namespace RC.Data.Repositories
{
    public class FuelingRepository(RCDbContext context) : IFuelingRepository
    {
        private readonly RCDbContext _context = context;

        public async Task<Fueling> AddAsync(Fueling newFueling)
        {
            await _context.AddAsync(newFueling);
            await _context.SaveChangesAsync();

            return newFueling;
        }

        public async Task<IEnumerable<Fueling>> GetAllAsync(int currentPage, int pageSize,
            long? organizationId = null, long? vehicleId = null, DateTime? from = null, DateTime? to = null)
        {
            return await ApplyFilters(_context.Set<Fueling>(), organizationId, vehicleId, from, to)
                .OrderByDescending(f => f.FueledAt)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetAllTotalAsync(long? organizationId = null, long? vehicleId = null,
            DateTime? from = null, DateTime? to = null)
        {
            return await ApplyFilters(_context.Set<Fueling>(), organizationId, vehicleId, from, to)
                .CountAsync();
        }

        private static IQueryable<Fueling> ApplyFilters(IQueryable<Fueling> query,
            long? organizationId, long? vehicleId, DateTime? from, DateTime? to)
        {
            return query
                .Where(f => organizationId == null || f.OrganizationId == organizationId)
                .Where(f => vehicleId == null || f.VehicleId == vehicleId)
                .Where(f => from == null || f.FueledAt >= from)
                .Where(f => to == null || f.FueledAt <= to);
        }

        public async Task<Fueling?> GetByIdAsync(long id)
        {
            return await _context.Set<Fueling>()
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<int?> GetLastMileageAsync(long vehicleId)
        {
            return await _context.Set<Fueling>()
                .Where(f => f.VehicleId == vehicleId)
                .MaxAsync(f => (int?)f.Mileage);
        }
    }
}

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

        public async Task<IEnumerable<Fueling>> GetAllAsync(int currentPage, int pageSize)
        {
            return await _context.Set<Fueling>()
                .OrderByDescending(f => f.FueledAt)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetAllTotalAsync()
        {
            return await _context.Set<Fueling>().CountAsync();
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

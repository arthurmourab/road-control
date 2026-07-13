using Microsoft.EntityFrameworkCore;
using RC.Data.Database;
using RC.Domain.Entities;
using RC.Domain.Interfaces.Repositories;

namespace RC.Data.Repositories
{
    public class GasStationRepository(RCDbContext context) : IGasStationRepository
    {
        private readonly RCDbContext _context = context;

        public async Task<GasStation> AddAsync(GasStation newGasStation)
        {
            await _context.AddAsync(newGasStation);
            await _context.SaveChangesAsync();

            return newGasStation;
        }

        public async Task<IEnumerable<GasStation>> GetAllAsync(int currentPage, int pageSize)
        {
            return await _context.Set<GasStation>()
                .Include(g => g.Organizations)
                .OrderBy(g => g.Id)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetAllTotalAsync()
        {
            return await _context.Set<GasStation>().CountAsync();
        }

        public async Task<IEnumerable<GasStation>> GetAvailableForOrganizationAsync(long? organizationId)
        {
            var query = _context.Set<GasStation>()
                .Include(g => g.Organizations)
                .Where(g => g.IsActive);

            // Restringe aos postos que atendem a organização (globais ou vinculados).
            // Sem organização (SystemAdmin), devolve todos os ativos.
            if (organizationId.HasValue)
                query = query.Where(g => g.IsGlobal
                    || g.Organizations.Any(o => o.OrganizationId == organizationId.Value));

            return await query.OrderBy(g => g.Id).ToListAsync();
        }

        public async Task<GasStation?> GetByIdAsync(long id)
        {
            return await _context.Set<GasStation>()
                .Include(g => g.Organizations)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<GasStation?> GetByDocumentAsync(string document)
        {
            return await _context.Set<GasStation>()
                .FirstOrDefaultAsync(g => g.Document == document);
        }

        public async Task UpdateAsync(GasStation gasStation)
        {
            _context.Update(gasStation);
            await _context.SaveChangesAsync();
        }
    }
}

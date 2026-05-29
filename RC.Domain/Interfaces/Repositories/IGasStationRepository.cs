using RC.Domain.Entities;

namespace RC.Domain.Interfaces.Repositories
{
    public interface IGasStationRepository
    {
        Task<GasStation> AddAsync(GasStation newGasStation);
        Task<IEnumerable<GasStation>> GetAllAsync(int currentPage, int pageSize);
        Task<int> GetAllTotalAsync();
        Task<GasStation?> GetByIdAsync(long id);
        Task<GasStation?> GetByDocumentAsync(string document);
        Task UpdateAsync(GasStation gasStation);
    }
}

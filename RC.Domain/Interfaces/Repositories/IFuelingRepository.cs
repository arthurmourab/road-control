using RC.Domain.Entities;

namespace RC.Domain.Interfaces.Repositories
{
    public interface IFuelingRepository
    {
        Task<Fueling> AddAsync(Fueling newFueling);
        Task<IEnumerable<Fueling>> GetAllAsync(int currentPage, int pageSize);
        Task<int> GetAllTotalAsync();
        Task<Fueling?> GetByIdAsync(long id);

        // Maior odômetro já registrado para o veículo (null se não houver abastecimentos)
        Task<int?> GetLastMileageAsync(long vehicleId);
    }
}

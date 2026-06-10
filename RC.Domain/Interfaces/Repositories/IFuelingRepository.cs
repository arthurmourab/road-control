using RC.Domain.Entities;

namespace RC.Domain.Interfaces.Repositories
{
    public interface IFuelingRepository
    {
        Task<Fueling> AddAsync(Fueling newFueling);
        Task<IEnumerable<Fueling>> GetAllAsync(int currentPage, int pageSize,
            long? organizationId = null, long? vehicleId = null, DateTime? from = null, DateTime? to = null);
        Task<int> GetAllTotalAsync(long? organizationId = null, long? vehicleId = null,
            DateTime? from = null, DateTime? to = null);
        Task<Fueling?> GetByIdAsync(long id);

        // Maior odômetro já registrado para o veículo (null se não houver abastecimentos)
        Task<int?> GetLastMileageAsync(long vehicleId);
    }
}

using RC.Domain.Entities;

namespace RC.Domain.Interfaces.Repositories
{
    public interface IGasStationRepository
    {
        Task<GasStation> AddAsync(GasStation newGasStation);
        Task<IEnumerable<GasStation>> GetAllAsync(int currentPage, int pageSize);
        // Postos ativos que atendem a organização (globais ou vinculados).
        // organizationId nulo (SystemAdmin sem contexto) retorna todos os ativos.
        Task<IEnumerable<GasStation>> GetAvailableForOrganizationAsync(long? organizationId);
        Task<int> GetAllTotalAsync();
        Task<GasStation?> GetByIdAsync(long id);
        Task<GasStation?> GetByDocumentAsync(string document);
        Task UpdateAsync(GasStation gasStation);
    }
}

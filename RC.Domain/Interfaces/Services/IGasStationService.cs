using RC.Shared.Dtos.GasStation;
using RC.Shared.Models.Results;

namespace RC.Domain.Interfaces.Services
{
    public interface IGasStationService
    {
        Task<PagedResult<GasStationDto>> GetAllAsync(int currentPage, int pageSize);
        Task<GasStationDto> AddAsync(NewGasStationDto newGasStationDto);
        Task<GasStationDto> GetByIdAsync(long id);
        Task<GasStationDto> UpdateAsync(long id, UpdateGasStationDto updateGasStationDto);
        Task<GasStationDto> LinkOrganizationsAsync(long gasStationId, IEnumerable<long> organizationIds);
        Task<GasStationDto> UnlinkOrganizationAsync(long gasStationId, long organizationId);
    }
}

using RC.Shared.Dtos.Fueling;
using RC.Shared.Models.Results;

namespace RC.Domain.Interfaces.Services
{
    public interface IFuelingService
    {
        Task<FuelingDto> RegisterAsync(NewFuelingDto newFuelingDto, long currentUserId, string? currentUserRole);
        Task<PagedResult<FuelingDto>> GetAllAsync(int currentPage, int pageSize);
        Task<FuelingDto> GetByIdAsync(long id);
    }
}

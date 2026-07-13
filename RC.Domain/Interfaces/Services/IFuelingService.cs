using RC.Shared.Dtos.Fueling;
using RC.Shared.Models.Results;

namespace RC.Domain.Interfaces.Services
{
    public interface IFuelingService
    {
        Task<FuelingDto> RegisterAsync(NewFuelingDto newFuelingDto, long currentUserId, string? currentUserRole);
        Task<PagedResult<FuelingDto>> GetAllAsync(int currentPage, int pageSize,
            long currentUserId, string? currentUserRole,
            long? organizationId, long? vehicleId, DateTime? from, DateTime? to, long? attendantId);
        Task<FuelingDto> GetByIdAsync(long id);

        // Código de confirmação corrente do frentista autenticado (provisiona o segredo se necessário)
        Task<ConfirmationCodeDto> GetConfirmationCodeAsync(long currentUserId);
    }
}

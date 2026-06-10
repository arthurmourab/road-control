using RC.Shared.Dtos;
using RC.Shared.Dtos.Vehicle;
using RC.Shared.Models.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace RC.Domain.Interfaces.Services
{
    public interface IVehicleService
    {
        Task<PagedResult<VehicleDto>> GetAllAsync(int currentPage, int pageSize,
            long currentUserId, string? currentUserRole, long? organizationId);
        Task<VehicleDto> AddNewAsync(NewVehicleDto newVehicleDto);
        Task<VehicleDto> UpdateAsync(long id, UpdateVehicleDto updateVehicleDto,
            long currentUserId, string? currentUserRole);
        Task<VehicleDto> SetActiveAsync(long id, bool isActive,
            long currentUserId, string? currentUserRole);
    }
}

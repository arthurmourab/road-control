using RC.Shared.Dtos;
using RC.Shared.Models.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace RC.Domain.Interfaces.Services
{
    public interface IVehicleService
    {
        Task<PagedResult<VehicleDto>> GetAllVehiclesAsync(int currentPage, int pageSize);
        Task<VehicleDto> AddNewVehicleAsync(NewVehicleDto newVehicleDto);
    }
}

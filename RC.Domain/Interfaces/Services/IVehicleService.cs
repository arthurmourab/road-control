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
        Task<PagedResult<VehicleDto>> GetAllAsync(int currentPage, int pageSize);
        Task<VehicleDto> AddNewAsync(NewVehicleDto newVehicleDto);
    }
}

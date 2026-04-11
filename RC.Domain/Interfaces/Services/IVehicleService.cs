using RC.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace RC.Domain.Interfaces.Services
{
    public interface IVehicleService
    {
        Task<IEnumerable<VehicleDto>> GetAllVehiclesAsync();
    }
}

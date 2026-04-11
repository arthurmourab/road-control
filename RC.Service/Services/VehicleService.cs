using RC.Domain.Interfaces.Services;
using RC.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace RC.Service.Services
{
    public class VehicleService : IVehicleService
    {
        public async Task<IEnumerable<VehicleDto>> GetAllVehiclesAsync()
        {
            return [new VehicleDto()];
        }
    }
}

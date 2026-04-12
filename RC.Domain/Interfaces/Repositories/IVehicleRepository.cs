using RC.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RC.Domain.Interfaces.Repositories
{
    public interface IVehicleRepository
    {
        Task<IEnumerable<Vehicle>> GetAllVehiclesAsync(int currentPage, int pageSize);
        Task<int> GetAllVehiclesNumber();
    }
}

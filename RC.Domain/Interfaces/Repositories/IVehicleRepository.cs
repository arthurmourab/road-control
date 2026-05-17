using RC.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RC.Domain.Interfaces.Repositories
{
    public interface IVehicleRepository
    {
        Task<IEnumerable<Vehicle>> GetAllAsync(int currentPage, int pageSize);
        Task<int> GetAllTotalAsync();
        Task<Vehicle> AddNewAsync(Vehicle newVehicle);
    }
}

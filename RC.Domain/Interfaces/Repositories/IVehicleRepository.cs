using RC.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RC.Domain.Interfaces.Repositories
{
    public interface IVehicleRepository
    {
        Task<IEnumerable<Vehicle>> GetAllAsync(int currentPage, int pageSize, long? organizationId = null);
        Task<int> GetAllTotalAsync(long? organizationId = null);
        Task<Vehicle> AddNewAsync(Vehicle newVehicle);
        Task<Vehicle?> GetByIdAsync(long id);
        Task<Vehicle?> GetByPlateAsync(string plate);
        Task UpdateAsync(Vehicle vehicle);
    }
}

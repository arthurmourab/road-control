using System;
using System.Collections.Generic;
using System.Text;

namespace RC.Domain.Interfaces.Repositories
{
    public interface IVehicleRepository
    {
        Task GetAllVehiclesAsync();
    }
}

using RC.Data.Database;
using RC.Domain.Entities;
using RC.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace RC.Data.Repositories
{
    public class VehicleRepository(RCDbContext context) : IVehicleRepository
    {
        private readonly RCDbContext _context = context;

        public async Task GetAllVehiclesAsync()
        {
            
        }
    }
}

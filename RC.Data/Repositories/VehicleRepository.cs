using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RC.Data.Database;
using RC.Domain.Entities;
using RC.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace RC.Data.Repositories
{
    public class VehicleRepository(RCDbContext context) : IVehicleRepository
    {
        private readonly RCDbContext _context = context;

        public async Task<IEnumerable<Vehicle>> GetAllVehiclesAsync(int currentPage, int pageSize)
        {
            return await _context.Set<Vehicle>()
                .OrderBy(x => x.Id)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();           
        }

        public async Task<int> GetAllVehiclesNumberAsync()
        {
            return await _context.Set<Vehicle>().CountAsync();
        }

        public async Task<Vehicle> AddNewVehicleAsync(Vehicle newVehicle)
        {
            await _context.AddAsync(newVehicle);
            await _context.SaveChangesAsync();

            return newVehicle;
        }


    }
}

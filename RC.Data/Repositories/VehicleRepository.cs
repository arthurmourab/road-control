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
            try
            {
                return await _context.Set<Vehicle>()
                                .OrderBy(x => x.Id)
                                .Skip((currentPage - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            
        }

        public async Task<int> GetAllVehiclesNumber()
        {
            return await _context.Set<Vehicle>().CountAsync();
        }
    }
}

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

        public async Task<IEnumerable<Vehicle>> GetAllAsync(int currentPage, int pageSize, long? organizationId = null)
        {
            return await _context.Set<Vehicle>()
                .Where(v => organizationId == null || v.OrganizationId == organizationId)
                .OrderBy(x => x.Id)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetAllTotalAsync(long? organizationId = null)
        {
            return await _context.Set<Vehicle>()
                .Where(v => organizationId == null || v.OrganizationId == organizationId)
                .CountAsync();
        }

        public async Task<Vehicle> AddNewAsync(Vehicle newVehicle)
        {
            await _context.AddAsync(newVehicle);
            await _context.SaveChangesAsync();

            return newVehicle;
        }

        public async Task<Vehicle?> GetByIdAsync(long id)
        {
            return await _context.Set<Vehicle>()
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<Vehicle?> GetByPlateAsync(string plate)
        {
            return await _context.Set<Vehicle>()
                .FirstOrDefaultAsync(v => v.Plate == plate);
        }

        public async Task UpdateAsync(Vehicle vehicle)
        {
            _context.Update(vehicle);
            await _context.SaveChangesAsync();
        }


    }
}

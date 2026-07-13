using Microsoft.EntityFrameworkCore;
using RC.Data.Database;
using RC.Domain.Entities;
using RC.Domain.Interfaces.Repositories;
using RC.Shared.Dtos.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace RC.Data.Repositories
{
    public class UserRepository(RCDbContext context) : IUserRepository
    {
        private readonly RCDbContext _context = context;

        public async Task<IEnumerable<User>> GetAllAsync(int currentPage, int pageSize, long? organizationId = null, long? gasStationId = null)
        {
            return await _context.Set<User>()
                .Include(u => u.Role)
                .Include(u => u.Organization)
                .Include(u => u.GasStation)
                .Where(u => organizationId == null || u.OrganizationId == organizationId)
                .Where(u => gasStationId == null || u.GasStationId == gasStationId)
                .OrderBy(u => u.Id)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetAllTotalAsync(long? organizationId = null, long? gasStationId = null)
        {
            return await _context.Set<User>()
                .Where(u => organizationId == null || u.OrganizationId == organizationId)
                .Where(u => gasStationId == null || u.GasStationId == gasStationId)
                .CountAsync();
        }

        public async Task<User?> GetByIdAsync(long id)
        {
            return await _context.Set<User>()
                .Include(u => u.Role)
                .Include(u => u.Organization)
                .Include(u => u.GasStation)
                .Where(u => u.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Set<User>()
                .Include(u => u.Role)
                .Include(u => u.Organization)
                .Include(u => u.GasStation)
                .Where(u => u.Email == email)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<User>> GetActiveAttendantsByStationAsync(long gasStationId)
        {
            return await _context.Set<User>()
                .Include(u => u.Role)
                .Include(u => u.Organization)
                .Include(u => u.GasStation)
                .Where(u => u.GasStationId == gasStationId
                    && u.IsActive
                    && u.Role.Name == Role.Roles.GasStationAttendant)
                .ToListAsync();
        }

        public async Task<User> AddAsync(User newUser)
        {
            await _context.AddAsync(newUser);
            await _context.SaveChangesAsync();

            return await _context.Set<User>()
                .Include(u => u.Role)
                .Include(u => u.Organization)
                .Include(u => u.GasStation)
                .FirstAsync(u => u.Id == newUser.Id);
        }

        public async Task<User?> DeactivateByIdAsync(long id)
        {
            var user = await GetByIdAsync(id);

            if (user is not null)
                user.IsActive = false;

            await _context.SaveChangesAsync();
            return user;
        }

        public async Task UpdateAsync(User user)
        {
            _context.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}

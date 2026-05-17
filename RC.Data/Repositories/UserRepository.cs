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

        public async Task<IEnumerable<User>> GetAllAsync(int currentPage, int pageSize)
        {
            return await _context.Set<User>()
                .OrderBy(u => u.Id)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetAllTotalAsync()
        {
            return await _context.Set<User>().CountAsync();
        }

        public async Task<User?> GetByIdAsync(long id)
        {
            return await _context.Set<User>()
                .Where(u => u.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Set<User>()
                .Where(u => u.Email == email)
                .FirstOrDefaultAsync();
        }

        public async Task<User> AddAsync(User newUser)
        {
            await _context.AddAsync(newUser);
            await _context.SaveChangesAsync();

            return newUser;
        }

        public async Task<User?> DeactivateByIdAsync(long id)
        {
            var user = await GetByIdAsync(id);
            
            user?.IsActive = false;

            await _context.SaveChangesAsync();
            return user;
        }
    }
}

using RC.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RC.Domain.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync(int currentPage, int pageSize);
        Task<User?> GetByIdAsync(long id);
        Task<User?> GetByEmailAsync(string email);
        Task<User> AddAsync(User newUser);
        Task<User?> DeactivateByIdAsync(long id);
    }
}

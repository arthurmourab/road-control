using RC.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RC.Domain.Interfaces.Repositories
{
    public interface IRoleRepository
    {
        Task<IEnumerable<Role>> GetAllAsync(int currentPage, int pageSize);
        Task<Role?> GetByIdAsync(long id);
    }
}

using RC.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RC.Domain.Interfaces.Repositories
{
    public interface IOrganizationRepository
    {
        Task<Organization> AddAsync(Organization newOrganization);
        Task<IEnumerable<Organization>> GetAllAsync(int currentPage, int pageSize);
        Task<int> GetAllTotalAsync();
        Task<Organization?> GetByIdAsync(long id);
    }
}

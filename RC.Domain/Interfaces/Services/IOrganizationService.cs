using RC.Shared.Dtos.Organization;
using RC.Shared.Models.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace RC.Domain.Interfaces.Services
{
    public interface IOrganizationService
    {
        Task<PagedResult<OrganizationDto>> GetAllAsync(int currentPage, int pageSize);
        Task<OrganizationDto> AddAsync(NewOrganizationDto newOrganizationDto);
        Task<OrganizationDto> GetByIdAsync(long id);
    }
}

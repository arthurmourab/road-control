using RC.Shared.Dtos.User;
using RC.Shared.Models.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace RC.Domain.Interfaces.Services
{
    public interface IUserService
    {
        Task<PagedResult<UserDto>> GetAllAsync(int currentPage, int pageSize,
            long currentUserId, string? currentUserRole, long? organizationId);
        Task<UserDto> GetByIdAsync(long id);
        Task<UserDto> AddAsync(NewUserDto newUser, long currentUserId, string? currentUserRole);
        Task<UserDto> SetActiveAsync(long id, bool isActive, long currentUserId, string? currentUserRole);
    }
}

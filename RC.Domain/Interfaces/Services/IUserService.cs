using RC.Shared.Dtos.User;
using RC.Shared.Models.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace RC.Domain.Interfaces.Services
{
    public interface IUserService
    {
        Task<PagedResult<UserDto>> GetAllAsync(int currentPage, int pageSize);
        Task<UserDto> AddAsync(NewUserDto newUser);
    }
}

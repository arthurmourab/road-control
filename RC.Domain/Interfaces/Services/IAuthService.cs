using RC.Shared.Dtos.Authentication;
using System;
using System.Collections.Generic;
using System.Text;

namespace RC.Domain.Interfaces.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequest);
    }
}

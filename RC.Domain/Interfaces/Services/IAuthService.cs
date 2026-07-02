using RC.Shared.Dtos.Authentication;
using System;
using System.Collections.Generic;
using System.Text;

namespace RC.Domain.Interfaces.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequest);
        Task ResetPasswordAsync(ResetPasswordDto resetPassword);
        Task ChangePasswordAsync(ChangePasswordDto changePassword, long currentUserId);
    }
}

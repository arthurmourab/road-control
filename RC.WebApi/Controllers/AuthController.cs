using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RC.Domain.Interfaces.Services;
using RC.Shared.Dtos.Authentication;
using RC.Shared.Dtos.User;
using RC.Shared.Models.Results;
using RC.WebApi.Extensions;

namespace RC.WebApi.Controllers
{
    [ApiController]
    [Route("/v1/[controller]")]
    public class AuthController(IAuthService authService, IUserService userService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;
        private readonly IUserService _userService = userService;

        [HttpPost]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequestDto loginRequest)
        {
            var response = await _authService.LoginAsync(loginRequest);
            return StatusCode(200, ApiResponse<LoginResponseDto>.Ok(response));
        }

        // Dados do usuário autenticado (inclui papel e organização) — usado pelo
        // frontend para montar o contexto da sessão após o login
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> MeAsync()
        {
            var response = await _userService.GetByIdAsync(User.GetUserId());
            return StatusCode(200, ApiResponse<UserDto>.Ok(response));
        }
    }
}

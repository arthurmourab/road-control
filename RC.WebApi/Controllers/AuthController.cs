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

        // Reset provisório de senha (sem fluxo de e-mail): define a senha como a provisória.
        // A resposta é sempre a mesma, exista o e-mail ou não, para evitar enumeração de contas.
        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPasswordDto resetPassword)
        {
            await _authService.ResetPasswordAsync(resetPassword);
            return StatusCode(200, ApiResponse<string>.Ok("If the email exists, the password has been reset."));
        }

        // Troca de senha do usuário autenticado (tela de perfil): opera sempre sobre
        // o próprio usuário do token — o id nunca vem do body.
        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordDto changePassword)
        {
            await _authService.ChangePasswordAsync(changePassword, User.GetUserId());
            return StatusCode(200, ApiResponse<string>.Ok("Password changed successfully."));
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

using Microsoft.AspNetCore.Mvc;
using RC.Domain.Interfaces.Services;
using RC.Shared.Dtos.Authentication;
using RC.Shared.Models.Results;

namespace RC.WebApi.Controllers
{
    [ApiController]
    [Route("/v1/[controller]")]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;
        [HttpPost]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequestDto loginRequest)
        {
            var response = await _authService.LoginAsync(loginRequest);
            return StatusCode(200, ApiResponse<LoginResponseDto>.Ok(response));
        }
    }
}

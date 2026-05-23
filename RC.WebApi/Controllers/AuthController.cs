using Microsoft.AspNetCore.Mvc;
using RC.Domain.Interfaces.Services;
using RC.Shared.Dtos.Authentication;

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
            try
            {
                var response = await _authService.LoginAsync(loginRequest);
                return StatusCode(200, response);
            }

            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }

            catch (Exception) 
            {
                return StatusCode(500, new { message = "Internal error" });
            }
            
        }
    }
}

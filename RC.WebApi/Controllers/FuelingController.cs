using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RC.Domain.Entities;
using RC.Domain.Interfaces.Services;
using RC.Shared.Dtos.Fueling;
using RC.Shared.Models.Results;
using System.Security.Claims;

namespace RC.WebApi.Controllers
{
    [ApiController]
    [Route("/v1/[controller]")]
    [Authorize(Roles = Role.Roles.FuelingManagers)]
    public class FuelingController(IFuelingService fuelingService) : ControllerBase
    {
        private readonly IFuelingService _fuelingService = fuelingService;

        [HttpPost]
        public async Task<IActionResult> RegisterAsync([FromBody] NewFuelingDto newFuelingDto)
        {
            var currentUserId = GetCurrentUserId();
            var currentUserRole = User.FindFirstValue(ClaimTypes.Role) ?? User.FindFirstValue("role");

            var response = await _fuelingService.RegisterAsync(newFuelingDto, currentUserId, currentUserRole);
            return StatusCode(201, ApiResponse<FuelingDto>.Ok(response));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] int currentPage = 1, int pageSize = 20)
        {
            var response = await _fuelingService.GetAllAsync(currentPage, pageSize);
            return StatusCode(200, ApiResponse<PagedResult<FuelingDto>>.Ok(response));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] long id)
        {
            var response = await _fuelingService.GetByIdAsync(id);
            return StatusCode(200, ApiResponse<FuelingDto>.Ok(response));
        }

        // Extrai o id do usuário autenticado do token (claim 'sub' / NameIdentifier)
        private long GetCurrentUserId()
        {
            var sub = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (!long.TryParse(sub, out var userId))
                throw new UnauthorizedAccessException("Invalid token: missing user id.");

            return userId;
        }
    }
}

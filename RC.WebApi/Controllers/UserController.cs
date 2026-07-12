using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RC.Domain.Entities;
using RC.Domain.Interfaces.Services;
using RC.Shared.Dtos.Common;
using RC.Shared.Dtos.User;
using RC.Shared.Models.Results;
using RC.WebApi.Extensions;

namespace RC.WebApi.Controllers
{
    [ApiController]
    [Route("/v1/[controller]")]
    public class UserController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        [Authorize(Roles = Role.Roles.UserManagers)]
        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody] NewUserDto newUser)
        {
            var response = await _userService.AddAsync(newUser, User.GetUserId(), User.GetRole());
            return StatusCode(201, ApiResponse<UserDto>.Ok(response));
        }
        
        // O service restringe a listagem ao escopo do chamador (organização ou posto)
        // (SystemAdmin enxerga tudo e pode filtrar via ?organizationId= e/ou ?gasStationId=)
        [Authorize(Roles = Role.Roles.UserManagers)]
        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] int currentPage = 1, int pageSize = 20,
            [FromQuery] long? organizationId = null, [FromQuery] long? gasStationId = null)
        {
            var response = await _userService.GetAllAsync(currentPage, pageSize,
                User.GetUserId(), User.GetRole(), organizationId, gasStationId);
            return StatusCode(200, ApiResponse<PagedResult<UserDto>>.Ok(response));
        }

        [Authorize(Roles = Role.Roles.UserManagers)]
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> SetStatusAsync([FromRoute] long id, [FromBody] UpdateStatusDto updateStatus)
        {
            var response = await _userService.SetActiveAsync(id, updateStatus.IsActive, User.GetUserId(), User.GetRole());
            return StatusCode(200, ApiResponse<UserDto>.Ok(response));
        }

        [Authorize(Roles = Role.Roles.SystemAdmin)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] long id)
        {
            var response = await _userService.GetByIdAsync(id);
            return StatusCode(200, ApiResponse<UserDto>.Ok(response));
        }
    }
}

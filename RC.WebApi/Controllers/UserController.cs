using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RC.Domain.Entities;
using RC.Domain.Interfaces.Services;
using RC.Shared.Dtos.User;
using RC.Shared.Models.Results;

namespace RC.WebApi.Controllers
{
    [ApiController]
    [Route("/v1/[controller]")]
    public class UserController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;
        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody] NewUserDto newUser)
        {
            var response = await _userService.AddAsync(newUser);
            return StatusCode(201, ApiResponse<UserDto>.Ok(response));            
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] int currentPage = 1, int pageSize = 20)
        {
            var response = await _userService.GetAllAsync(currentPage, pageSize);
            return StatusCode(200, ApiResponse<PagedResult<UserDto>>.Ok(response));
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

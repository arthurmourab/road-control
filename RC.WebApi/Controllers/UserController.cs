using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RC.Domain.Entities;
using RC.Domain.Interfaces.Services;
using RC.Shared.Dtos.User;
using System.Diagnostics.Eventing.Reader;

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
            try
            {
                var response = await _userService.AddAsync(newUser);
                return StatusCode(201, response);
            }

            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        
        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] int currentPage = 1, int pageSize = 20)
        {
            try
            {
                var response = await _userService.GetAllAsync(currentPage, pageSize);
                return Ok(response);
            }

            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        [Authorize(Roles = Role.Roles.SystemAdmin)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] long id)
        {
            try
            {
                var response = await _userService.GetByIdAsync(id);
                return Ok(response);
            }

            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}

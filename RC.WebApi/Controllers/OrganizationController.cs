using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RC.Domain.Entities;
using RC.Domain.Interfaces.Services;
using RC.Shared.Dtos.Organization;
using RC.Shared.Models.Results;

namespace RC.WebApi.Controllers
{
    [ApiController]
    [Route("/v1/[controller]")]
    public class OrganizationController(IOrganizationService organizationService) : ControllerBase
    {
        private readonly IOrganizationService _organizationService = organizationService;

        [Authorize(Roles = Role.Roles.SystemAdmin)]
        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody] NewOrganizationDto newOrganizationDto)
        {
            var response = await _organizationService.AddAsync(newOrganizationDto);
            return StatusCode(201, ApiResponse<OrganizationDto>.Ok(response));
        }

        // Só a administração da plataforma enxerga a carteira de organizações;
        // usuários de organização obtêm os dados da própria org via /v1/auth/me
        [Authorize(Roles = Role.Roles.SystemAdmin)]
        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] int currentPage = 1, int pageSize = 20)
        {
            var response = await _organizationService.GetAllAsync(currentPage, pageSize);
            return StatusCode(200, ApiResponse<PagedResult<OrganizationDto>>.Ok(response));
        }

        [Authorize(Roles = Role.Roles.SystemAdmin)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] long id)
        {
            var response = await _organizationService.GetByIdAsync(id);
            return StatusCode(200, ApiResponse<OrganizationDto>.Ok(response));
        }
    }
}

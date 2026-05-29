using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RC.Domain.Entities;
using RC.Domain.Interfaces.Services;
using RC.Shared.Dtos.GasStation;
using RC.Shared.Models.Results;

namespace RC.WebApi.Controllers
{
    [ApiController]
    [Route("/v1/[controller]")]
    [Authorize(Roles = Role.Roles.SystemAdmin)]
    public class GasStationController(IGasStationService gasStationService) : ControllerBase
    {
        private readonly IGasStationService _gasStationService = gasStationService;

        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody] NewGasStationDto newGasStationDto)
        {
            var response = await _gasStationService.AddAsync(newGasStationDto);
            return StatusCode(201, ApiResponse<GasStationDto>.Ok(response));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] int currentPage = 1, int pageSize = 20)
        {
            var response = await _gasStationService.GetAllAsync(currentPage, pageSize);
            return StatusCode(200, ApiResponse<PagedResult<GasStationDto>>.Ok(response));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] long id)
        {
            var response = await _gasStationService.GetByIdAsync(id);
            return StatusCode(200, ApiResponse<GasStationDto>.Ok(response));
        }

        [HttpPost("{id}/organizations")]
        public async Task<IActionResult> LinkOrganizationsAsync([FromRoute] long id, [FromBody] LinkOrganizationsDto linkOrganizationsDto)
        {
            var response = await _gasStationService.LinkOrganizationsAsync(id, linkOrganizationsDto.OrganizationIds);
            return StatusCode(200, ApiResponse<GasStationDto>.Ok(response));
        }

        [HttpDelete("{id}/organizations/{organizationId}")]
        public async Task<IActionResult> UnlinkOrganizationAsync([FromRoute] long id, [FromRoute] long organizationId)
        {
            var response = await _gasStationService.UnlinkOrganizationAsync(id, organizationId);
            return StatusCode(200, ApiResponse<GasStationDto>.Ok(response));
        }
    }
}

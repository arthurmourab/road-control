using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RC.Domain.Entities;
using RC.Domain.Interfaces.Services;
using RC.Shared.Dtos.GasStation;
using RC.Shared.Models.Results;
using RC.WebApi.Extensions;

namespace RC.WebApi.Controllers
{
    [ApiController]
    [Route("/v1/[controller]")]
    public class GasStationController(IGasStationService gasStationService) : ControllerBase
    {
        private readonly IGasStationService _gasStationService = gasStationService;

        // Postos disponíveis para a organização do chamador (motorista/gestor escolhem onde abastecer).
        // O service restringe aos postos ativos que atendem a organização do chamador.
        [Authorize(Roles = Role.Roles.GasStationViewers)]
        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableAsync()
        {
            var response = await _gasStationService.GetAvailableAsync(User.GetUserId(), User.GetRole());
            return StatusCode(200, ApiResponse<IEnumerable<GasStationDto>>.Ok(response));
        }

        [Authorize(Roles = Role.Roles.SystemAdmin)]
        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody] NewGasStationDto newGasStationDto)
        {
            var response = await _gasStationService.AddAsync(newGasStationDto);
            return StatusCode(201, ApiResponse<GasStationDto>.Ok(response));
        }

        [Authorize(Roles = Role.Roles.SystemAdmin)]
        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] int currentPage = 1, int pageSize = 20)
        {
            var response = await _gasStationService.GetAllAsync(currentPage, pageSize);
            return StatusCode(200, ApiResponse<PagedResult<GasStationDto>>.Ok(response));
        }

        [Authorize(Roles = Role.Roles.SystemAdmin)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] long id)
        {
            var response = await _gasStationService.GetByIdAsync(id);
            return StatusCode(200, ApiResponse<GasStationDto>.Ok(response));
        }

        [Authorize(Roles = Role.Roles.SystemAdmin)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] long id, [FromBody] UpdateGasStationDto updateGasStationDto)
        {
            var response = await _gasStationService.UpdateAsync(id, updateGasStationDto);
            return StatusCode(200, ApiResponse<GasStationDto>.Ok(response));
        }

        [Authorize(Roles = Role.Roles.SystemAdmin)]
        [HttpPost("{id}/organizations")]
        public async Task<IActionResult> LinkOrganizationsAsync([FromRoute] long id, [FromBody] LinkOrganizationsDto linkOrganizationsDto)
        {
            var response = await _gasStationService.LinkOrganizationsAsync(id, linkOrganizationsDto.OrganizationIds);
            return StatusCode(200, ApiResponse<GasStationDto>.Ok(response));
        }

        [Authorize(Roles = Role.Roles.SystemAdmin)]
        [HttpDelete("{id}/organizations/{organizationId}")]
        public async Task<IActionResult> UnlinkOrganizationAsync([FromRoute] long id, [FromRoute] long organizationId)
        {
            var response = await _gasStationService.UnlinkOrganizationAsync(id, organizationId);
            return StatusCode(200, ApiResponse<GasStationDto>.Ok(response));
        }
    }
}

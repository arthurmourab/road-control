using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RC.Domain.Entities;
using RC.Domain.Interfaces.Services;
using RC.Shared.Dtos.Common;
using RC.Shared.Dtos.Vehicle;
using RC.Shared.Models.Results;
using RC.WebApi.Extensions;

namespace RC.WebApi.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    public class VehicleController(IVehicleService vehicleService) : ControllerBase
    {
        private readonly IVehicleService _vehicleService = vehicleService;

        // Qualquer usuário autenticado lista veículos; o service restringe à organização
        // do chamador (SystemAdmin enxerga tudo e pode filtrar via ?organizationId=)
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] int currentPage = 1, int pageSize = 20,
            [FromQuery] long? organizationId = null)
        {
            var response = await _vehicleService.GetAllAsync(currentPage, pageSize,
                User.GetUserId(), User.GetRole(), organizationId);
            return StatusCode(200, ApiResponse<PagedResult<VehicleDto>>.Ok(response));
        }

        [Authorize(Roles = Role.Roles.FleetManagers)]
        [HttpPost]
        public async Task<IActionResult> AddNewAsync([FromBody] NewVehicleDto newVehicle)
        {
            var response = await _vehicleService.AddNewAsync(newVehicle);
            return StatusCode(201, ApiResponse<VehicleDto>.Ok(response));
        }

        [Authorize(Roles = Role.Roles.FleetManagers)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] long id, [FromBody] UpdateVehicleDto updateVehicle)
        {
            var response = await _vehicleService.UpdateAsync(id, updateVehicle, User.GetUserId(), User.GetRole());
            return StatusCode(200, ApiResponse<VehicleDto>.Ok(response));
        }

        [Authorize(Roles = Role.Roles.FleetManagers)]
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> SetStatusAsync([FromRoute] long id, [FromBody] UpdateStatusDto updateStatus)
        {
            var response = await _vehicleService.SetActiveAsync(id, updateStatus.IsActive, User.GetUserId(), User.GetRole());
            return StatusCode(200, ApiResponse<VehicleDto>.Ok(response));
        }
    }
}

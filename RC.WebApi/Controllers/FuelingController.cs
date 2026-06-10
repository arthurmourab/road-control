using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RC.Domain.Entities;
using RC.Domain.Interfaces.Services;
using RC.Shared.Dtos.Fueling;
using RC.Shared.Models.Results;
using RC.WebApi.Extensions;

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
            var response = await _fuelingService.RegisterAsync(newFuelingDto, User.GetUserId(), User.GetRole());
            return StatusCode(201, ApiResponse<FuelingDto>.Ok(response));
        }

        // O service restringe a listagem à organização do chamador
        // (SystemAdmin enxerga tudo e pode filtrar via ?organizationId=).
        // Filtros opcionais: ?vehicleId=, ?from=, ?to= (período do abastecimento)
        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] int currentPage = 1, int pageSize = 20,
            [FromQuery] long? organizationId = null, [FromQuery] long? vehicleId = null,
            [FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        {
            var response = await _fuelingService.GetAllAsync(currentPage, pageSize,
                User.GetUserId(), User.GetRole(), organizationId, vehicleId, from, to);
            return StatusCode(200, ApiResponse<PagedResult<FuelingDto>>.Ok(response));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] long id)
        {
            var response = await _fuelingService.GetByIdAsync(id);
            return StatusCode(200, ApiResponse<FuelingDto>.Ok(response));
        }
    }
}

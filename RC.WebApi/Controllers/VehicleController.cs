using Microsoft.AspNetCore.Mvc;
using RC.Domain.Interfaces.Services;
using RC.Shared.Dtos.Vehicle;
using RC.Shared.Models.Results;

namespace RC.WebApi.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    public class VehicleController(IVehicleService vehicleService) : ControllerBase
    {
        private readonly IVehicleService _vehicleService = vehicleService;

        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] int currentPage = 1, int pageSize = 20)
        {
            var response =  await _vehicleService.GetAllAsync(currentPage, pageSize);
            return StatusCode(200, ApiResponse<PagedResult<VehicleDto>>.Ok(response));
        }

        [HttpPost]
        public async Task<IActionResult> AddNewAsync([FromBody] NewVehicleDto newVehicle)
        {
            var response = await _vehicleService.AddNewAsync(newVehicle);
            return StatusCode(201, ApiResponse<VehicleDto>.Ok(response));
        }

    }
}

using Microsoft.AspNetCore.Mvc;
using RC.Domain.Interfaces.Services;
using RC.Shared.Dtos.Vehicle;

namespace RC.WebApi.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    public class VehicleController(IVehicleService vehicleService) : Controller
    {
        private readonly IVehicleService _vehicleService = vehicleService;

        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] int currentPage = 1, int pageSize = 20)
        {

            try
            {
                var response =  await _vehicleService.GetAllAsync(currentPage, pageSize);
                return Ok(response);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
            
        }

        [HttpPost]
        public async Task<IActionResult> AddNewAsync([FromBody] NewVehicleDto newVehicle)
        {
            try
            {
                var response = await _vehicleService.AddNewAsync(newVehicle);
                return StatusCode(StatusCodes.Status201Created, response);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

    }
}

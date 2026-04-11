using Microsoft.AspNetCore.Mvc;
using RC.Domain.Interfaces.Services;
using RC.Shared.Dtos;

namespace RC.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VehicleController(IVehicleService vehicleService) : Controller
    {
        private readonly IVehicleService _vehicleService = vehicleService;

        [HttpGet]
        public async Task<IEnumerable<VehicleDto>>GetAllVehiclesAsync()
        {

            return await _vehicleService.GetAllVehiclesAsync();
        }

    }
}

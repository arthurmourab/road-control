using RC.Domain.Entities;
using RC.Domain.Interfaces.Repositories;
using RC.Domain.Interfaces.Services;
using RC.Shared.Dtos;
using RC.Shared.Models.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace RC.Service.Services
{
    public class VehicleService(IVehicleRepository vehicleRepository) : IVehicleService
    {
        private readonly IVehicleRepository _vehicleRepository = vehicleRepository;

        public async Task<PagedResult<VehicleDto>> GetAllVehiclesAsync(int currentPage, int pageSize)
        {
            var vehicles = await _vehicleRepository.GetAllVehiclesAsync(currentPage, pageSize);
            var vehiclesNumber = await _vehicleRepository.GetAllVehiclesNumber();

            return new PagedResult<VehicleDto>
            {
                CurrentPage = currentPage,
                PageSize = pageSize,
                TotalRows = vehiclesNumber,
                Results = MapVehicleEntityToDto(vehicles)
            };
        }

        private IEnumerable<VehicleDto> MapVehicleEntityToDto(IEnumerable<Vehicle> vehicles)
        {
            return vehicles.Select(v => new VehicleDto
            {
                Id = v.Id,
                Type = v.Type,
                Plate = v.Plate,
                Brand = v.Brand,
                Model = v.Model,
                YearManufacture = v.YearManufacture,
                YearModel = v.YearModel,
                Mileage = v.Mileage,

            }).ToList();
        }
    }
}

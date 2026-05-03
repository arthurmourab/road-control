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
            var vehiclesNumber = await _vehicleRepository.GetAllVehiclesNumberAsync();

            return new PagedResult<VehicleDto>
            {
                CurrentPage = currentPage,
                PageSize = pageSize,
                TotalRows = vehiclesNumber,
                Results = MapVehicleEntityListToDtoList(vehicles)
            };
        }

        public async Task<VehicleDto> AddNewVehicleAsync(VehicleDto newVehicleDto)
        {
            var newVehicle = MapVehicleDtoToEntity(newVehicleDto);

            var insertedVehicle = await _vehicleRepository.AddNewVehicleAsync(newVehicle);

            var insertedVehicleDto = MapVehicleEntityToDto(insertedVehicle);
            return insertedVehicleDto;
        }

        private VehicleDto MapVehicleEntityToDto(Vehicle vehicle)
        {
            return new VehicleDto
            {
                Id = vehicle.Id,
                Type = vehicle.Type,
                Plate = vehicle.Plate,
                Brand = vehicle.Brand,
                Model = vehicle.Model,
                YearManufacture = vehicle.YearManufacture,
                YearModel = vehicle.YearModel,
                Mileage = vehicle.Mileage,
                IsActive = vehicle.IsActive,
                CreatedAt = vehicle.CreatedAt,
                UpdatedAt = vehicle.UpdatedAt,
            };
        }

        private Vehicle MapVehicleDtoToEntity(VehicleDto vehicle)
        {
            return new Vehicle
            {
                Type = vehicle.Type,
                Plate = vehicle.Plate,
                Brand = vehicle.Brand,
                Model = vehicle.Model,
                YearManufacture = vehicle.YearManufacture,
                YearModel = vehicle.YearModel,
                Mileage = vehicle.Mileage,
                IsActive = vehicle.IsActive ?? true,
            };
        }

        private IEnumerable<VehicleDto> MapVehicleEntityListToDtoList(IEnumerable<Vehicle> vehicles)
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
                IsActive = v.IsActive,
                CreatedAt = v.CreatedAt,
                UpdatedAt= v.UpdatedAt
            }).ToList();
        }

        private IEnumerable<Vehicle> MapVehicleDtoListToEntityList (IEnumerable<VehicleDto> vehicles)
        {
            return vehicles.Select(v => new Vehicle
            {
                Type = v.Type,
                Plate = v.Plate,
                Brand = v.Brand,
                Model = v.Model,
                YearManufacture = v.YearManufacture,
                YearModel = v.YearModel,
                Mileage = v.Mileage,
                IsActive = v.IsActive ?? true,
            }).ToList();
        }
    }
}

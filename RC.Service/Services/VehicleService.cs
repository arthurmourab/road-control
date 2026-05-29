using RC.Domain.Entities;
using RC.Domain.Exceptions;
using RC.Domain.Interfaces.Repositories;
using RC.Domain.Interfaces.Services;
using RC.Shared.Dtos.Vehicle;
using RC.Shared.Models.Results;

namespace RC.Service.Services
{
    public class VehicleService(
        IVehicleRepository vehicleRepository,
        IOrganizationRepository organizationRepository) : IVehicleService
    {
        private readonly IVehicleRepository _vehicleRepository = vehicleRepository;
        private readonly IOrganizationRepository _organizationRepository = organizationRepository;

        public async Task<PagedResult<VehicleDto>> GetAllAsync(int currentPage, int pageSize)
        {
            var vehicles = await _vehicleRepository.GetAllAsync(currentPage, pageSize);
            var vehiclesTotal = await _vehicleRepository.GetAllTotalAsync();

            return new PagedResult<VehicleDto>
            {
                CurrentPage = currentPage,
                PageSize = pageSize,
                TotalRows = vehiclesTotal,
                Results = MapVehicleListToVehicleDtoList(vehicles)
            };
        }

        public async Task<VehicleDto> AddNewAsync(NewVehicleDto newVehicleDto)
        {
            _ = await _organizationRepository.GetByIdAsync(newVehicleDto.OrganizationId)
                ?? throw new NotFoundException("Organization not found");

            var newVehicle = MapNewVehicleDtoToEntity(newVehicleDto);

            var insertedVehicle = await _vehicleRepository.AddNewAsync(newVehicle);

            var insertedVehicleDto = MapVehicleEntityToVehicleDto(insertedVehicle);
            return insertedVehicleDto;
        }

        private VehicleDto MapVehicleEntityToVehicleDto(Vehicle vehicle)
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
                OrganizationId = vehicle.OrganizationId,
            };
        }

        private Vehicle MapNewVehicleDtoToEntity(NewVehicleDto vehicle)
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
                IsActive = true,
                OrganizationId = vehicle.OrganizationId,
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
                OrganizationId = vehicle.OrganizationId,
            };
        }


        private IEnumerable<VehicleDto> MapVehicleListToVehicleDtoList(IEnumerable<Vehicle> vehicles)
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
                UpdatedAt= v.UpdatedAt,
                OrganizationId = v.OrganizationId
            }).ToList();
        }

        private IEnumerable<Vehicle> MapVehicleDtoListToList (IEnumerable<VehicleDto> vehicles)
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
                OrganizationId = v.OrganizationId,
            }).ToList();
        }
    }
}

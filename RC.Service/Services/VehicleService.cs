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
        IOrganizationRepository organizationRepository,
        IUserRepository userRepository) : IVehicleService
    {
        private readonly IVehicleRepository _vehicleRepository = vehicleRepository;
        private readonly IOrganizationRepository _organizationRepository = organizationRepository;
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<PagedResult<VehicleDto>> GetAllAsync(int currentPage, int pageSize,
            long currentUserId, string? currentUserRole, long? organizationId)
        {
            var scopeOrganizationId = await ResolveOrganizationScopeAsync(currentUserId, currentUserRole, organizationId);

            var vehicles = await _vehicleRepository.GetAllAsync(currentPage, pageSize, scopeOrganizationId);
            var vehiclesTotal = await _vehicleRepository.GetAllTotalAsync(scopeOrganizationId);

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

            var plateExists = await _vehicleRepository.GetByPlateAsync(newVehicleDto.Plate);
            if (plateExists != null) throw new ConflictException("Plate already registred.");

            var newVehicle = MapNewVehicleDtoToEntity(newVehicleDto);

            var insertedVehicle = await _vehicleRepository.AddNewAsync(newVehicle);

            var insertedVehicleDto = MapVehicleEntityToVehicleDto(insertedVehicle);
            return insertedVehicleDto;
        }

        public async Task<VehicleDto> UpdateAsync(long id, UpdateVehicleDto updateVehicleDto,
            long currentUserId, string? currentUserRole)
        {
            var vehicle = await GetScopedVehicleAsync(id, currentUserId, currentUserRole);

            // Se a placa mudou, garante que não conflita com outro veículo
            if (!string.Equals(vehicle.Plate, updateVehicleDto.Plate, StringComparison.OrdinalIgnoreCase))
            {
                var plateOwner = await _vehicleRepository.GetByPlateAsync(updateVehicleDto.Plate);
                if (plateOwner != null && plateOwner.Id != vehicle.Id)
                    throw new ConflictException("Plate already registred.");
            }

            vehicle.Type = updateVehicleDto.Type;
            vehicle.Plate = updateVehicleDto.Plate;
            vehicle.Brand = updateVehicleDto.Brand;
            vehicle.Model = updateVehicleDto.Model;
            vehicle.YearManufacture = updateVehicleDto.YearManufacture;
            vehicle.YearModel = updateVehicleDto.YearModel;
            vehicle.Mileage = updateVehicleDto.Mileage;

            await _vehicleRepository.UpdateAsync(vehicle);
            return MapVehicleEntityToVehicleDto(vehicle);
        }

        public async Task<VehicleDto> SetActiveAsync(long id, bool isActive,
            long currentUserId, string? currentUserRole)
        {
            var vehicle = await GetScopedVehicleAsync(id, currentUserId, currentUserRole);

            vehicle.IsActive = isActive;
            await _vehicleRepository.UpdateAsync(vehicle);

            return MapVehicleEntityToVehicleDto(vehicle);
        }

        // Busca o veículo garantindo que o chamador tem acesso a ele.
        // Responde 404 (e não 403) para não revelar a existência de veículos de outras organizações.
        private async Task<Vehicle> GetScopedVehicleAsync(long id, long currentUserId, string? currentUserRole)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(id)
                ?? throw new NotFoundException("Vehicle not found");

            var scopeOrganizationId = await ResolveOrganizationScopeAsync(currentUserId, currentUserRole, null);
            if (scopeOrganizationId.HasValue && vehicle.OrganizationId != scopeOrganizationId.Value)
                throw new NotFoundException("Vehicle not found");

            return vehicle;
        }

        // Resolve o escopo de organização do chamador:
        // SystemAdmin enxerga tudo (podendo filtrar por uma organização específica);
        // os demais papéis ficam restritos à própria organização.
        private async Task<long?> ResolveOrganizationScopeAsync(long currentUserId, string? currentUserRole, long? requestedOrganizationId)
        {
            if (currentUserRole == Role.Roles.SystemAdmin)
                return requestedOrganizationId;

            var currentUser = await _userRepository.GetByIdAsync(currentUserId)
                ?? throw new NotFoundException("Current user not found");

            if (!currentUser.OrganizationId.HasValue)
                throw new BusinessRuleException("User must belong to an organization.");

            return currentUser.OrganizationId;
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

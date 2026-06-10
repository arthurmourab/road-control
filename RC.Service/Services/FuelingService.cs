using RC.Domain.Entities;
using RC.Domain.Exceptions;
using RC.Domain.Interfaces.Repositories;
using RC.Domain.Interfaces.Services;
using RC.Shared.Dtos.Fueling;
using RC.Shared.Models.Results;

namespace RC.Service.Services
{
    public class FuelingService(
        IFuelingRepository fuelingRepository,
        IVehicleRepository vehicleRepository,
        IGasStationRepository gasStationRepository,
        IUserRepository userRepository) : IFuelingService
    {
        private readonly IFuelingRepository _fuelingRepository = fuelingRepository;
        private readonly IVehicleRepository _vehicleRepository = vehicleRepository;
        private readonly IGasStationRepository _gasStationRepository = gasStationRepository;
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<FuelingDto> RegisterAsync(NewFuelingDto newFuelingDto, long currentUserId, string? currentUserRole)
        {
            // Valores devem ser positivos
            if (newFuelingDto.Liters <= 0 || newFuelingDto.PricePerLiter <= 0)
                throw new BusinessRuleException("Liters and price per liter must be greater than zero.");

            var vehicle = await _vehicleRepository.GetByIdAsync(newFuelingDto.VehicleId)
                ?? throw new NotFoundException("Vehicle not found");

            // Resolve o motorista: Driver usa o próprio id do token; admins informam no body
            var driverId = ResolveDriverId(newFuelingDto, currentUserId, currentUserRole);

            var driver = await _userRepository.GetByIdAsync(driverId)
                ?? throw new NotFoundException("Driver not found");

            // Motorista deve pertencer à organização do veículo
            if (driver.OrganizationId != vehicle.OrganizationId)
                throw new BusinessRuleException("Driver does not belong to the vehicle's organization.");

            var gasStation = await _gasStationRepository.GetByIdAsync(newFuelingDto.GasStationId)
                ?? throw new NotFoundException("Gas station not found");

            // Posto deve ser global ou estar vinculado à organização do veículo
            var stationServesOrganization = gasStation.IsGlobal
                || gasStation.Organizations.Any(o => o.OrganizationId == vehicle.OrganizationId);
            if (!stationServesOrganization)
                throw new BusinessRuleException("Gas station does not serve the vehicle's organization.");

            // Odômetro não pode retroceder em relação ao último abastecimento
            var lastMileage = await _fuelingRepository.GetLastMileageAsync(vehicle.Id);
            if (lastMileage.HasValue && newFuelingDto.Mileage < lastMileage.Value)
                throw new BusinessRuleException($"Mileage cannot be lower than the last record ({lastMileage.Value}).");

            var fueling = new Fueling
            {
                VehicleId = vehicle.Id,
                GasStationId = gasStation.Id,
                DriverId = driverId,
                OrganizationId = vehicle.OrganizationId,
                FuelType = newFuelingDto.FuelType,
                Liters = newFuelingDto.Liters,
                PricePerLiter = newFuelingDto.PricePerLiter,
                TotalAmount = newFuelingDto.Liters * newFuelingDto.PricePerLiter,
                Mileage = newFuelingDto.Mileage,
                FueledAt = newFuelingDto.FueledAt
            };

            var inserted = await _fuelingRepository.AddAsync(fueling);
            return MapFuelingToDto(inserted);
        }

        public async Task<PagedResult<FuelingDto>> GetAllAsync(int currentPage, int pageSize,
            long currentUserId, string? currentUserRole,
            long? organizationId, long? vehicleId, DateTime? from, DateTime? to)
        {
            var scopeOrganizationId = await ResolveOrganizationScopeAsync(currentUserId, currentUserRole, organizationId);

            var fuelings = await _fuelingRepository.GetAllAsync(currentPage, pageSize, scopeOrganizationId, vehicleId, from, to);
            var total = await _fuelingRepository.GetAllTotalAsync(scopeOrganizationId, vehicleId, from, to);

            return new PagedResult<FuelingDto>
            {
                CurrentPage = currentPage,
                PageSize = pageSize,
                TotalRows = total,
                Results = fuelings.Select(MapFuelingToDto).ToList()
            };
        }

        public async Task<FuelingDto> GetByIdAsync(long id)
        {
            var fueling = await _fuelingRepository.GetByIdAsync(id) ?? throw new NotFoundException("Fueling not found");
            return MapFuelingToDto(fueling);
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

        private static long ResolveDriverId(NewFuelingDto dto, long currentUserId, string? currentUserRole)
        {
            // O motorista registra o próprio abastecimento
            if (currentUserRole == Role.Roles.Driver)
                return currentUserId;

            // OrganizationAdmin/SystemAdmin lançam em nome de um motorista
            if (!dto.DriverId.HasValue)
                throw new BusinessRuleException("DriverId is required when registering on behalf of a driver.");

            return dto.DriverId.Value;
        }

        private FuelingDto MapFuelingToDto(Fueling f)
        {
            return new FuelingDto
            {
                Id = f.Id,
                CreatedAt = f.CreatedAt,
                UpdatedAt = f.UpdatedAt,
                VehicleId = f.VehicleId,
                GasStationId = f.GasStationId,
                DriverId = f.DriverId,
                OrganizationId = f.OrganizationId,
                FuelType = f.FuelType,
                Liters = f.Liters,
                PricePerLiter = f.PricePerLiter,
                TotalAmount = f.TotalAmount,
                Mileage = f.Mileage,
                FueledAt = f.FueledAt
            };
        }
    }
}

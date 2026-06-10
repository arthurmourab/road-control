using RC.Domain.Entities;
using RC.Domain.Exceptions;
using RC.Domain.Interfaces.Repositories;
using RC.Domain.Interfaces.Services;
using RC.Shared.Dtos.GasStation;
using RC.Shared.Models.Results;

namespace RC.Service.Services
{
    public class GasStationService(
        IGasStationRepository gasStationRepository,
        IOrganizationRepository organizationRepository) : IGasStationService
    {
        private readonly IGasStationRepository _gasStationRepository = gasStationRepository;
        private readonly IOrganizationRepository _organizationRepository = organizationRepository;

        public async Task<PagedResult<GasStationDto>> GetAllAsync(int currentPage, int pageSize)
        {
            var gasStations = await _gasStationRepository.GetAllAsync(currentPage, pageSize);
            var total = await _gasStationRepository.GetAllTotalAsync();

            return new PagedResult<GasStationDto>
            {
                CurrentPage = currentPage,
                PageSize = pageSize,
                TotalRows = total,
                Results = MapGasStationListToDtoList(gasStations)
            };
        }

        public async Task<GasStationDto> AddAsync(NewGasStationDto newGasStationDto)
        {
            var newGasStation = MapNewDtoToGasStation(newGasStationDto);

            var exists = await _gasStationRepository.GetByDocumentAsync(newGasStation.Document);
            if (exists != null) throw new ConflictException("Gas station already registred.");

            var gasStation = await _gasStationRepository.AddAsync(newGasStation);
            return MapGasStationToDto(gasStation);
        }

        public async Task<GasStationDto> GetByIdAsync(long id)
        {
            var gasStation = await _gasStationRepository.GetByIdAsync(id) ?? throw new NotFoundException("Gas station not found");
            return MapGasStationToDto(gasStation);
        }

        public async Task<GasStationDto> UpdateAsync(long id, UpdateGasStationDto updateGasStationDto)
        {
            var gasStation = await _gasStationRepository.GetByIdAsync(id) ?? throw new NotFoundException("Gas station not found");

            gasStation.Name = updateGasStationDto.Name;
            gasStation.IsGlobal = updateGasStationDto.IsGlobal;
            gasStation.IsActive = updateGasStationDto.IsActive;
            gasStation.Street = updateGasStationDto.Street;
            gasStation.Number = updateGasStationDto.Number;
            gasStation.Neighborhood = updateGasStationDto.Neighborhood;
            gasStation.City = updateGasStationDto.City;
            gasStation.State = updateGasStationDto.State;
            gasStation.ZipCode = updateGasStationDto.ZipCode;

            await _gasStationRepository.UpdateAsync(gasStation);
            return MapGasStationToDto(gasStation);
        }

        public async Task<GasStationDto> LinkOrganizationsAsync(long gasStationId, IEnumerable<long> organizationIds)
        {
            var gasStation = await _gasStationRepository.GetByIdAsync(gasStationId) ?? throw new NotFoundException("Gas station not found");

            foreach (var organizationId in organizationIds.Distinct())
            {
                _ = await _organizationRepository.GetByIdAsync(organizationId)
                    ?? throw new NotFoundException($"Organization {organizationId} not found");

                var alreadyLinked = gasStation.Organizations.Any(o => o.OrganizationId == organizationId);
                if (!alreadyLinked)
                {
                    gasStation.Organizations.Add(new OrganizationGasStation
                    {
                        GasStationId = gasStationId,
                        OrganizationId = organizationId
                    });
                }
            }

            await _gasStationRepository.UpdateAsync(gasStation);
            return MapGasStationToDto(gasStation);
        }

        public async Task<GasStationDto> UnlinkOrganizationAsync(long gasStationId, long organizationId)
        {
            var gasStation = await _gasStationRepository.GetByIdAsync(gasStationId) ?? throw new NotFoundException("Gas station not found");

            var link = gasStation.Organizations.FirstOrDefault(o => o.OrganizationId == organizationId);
            if (link != null)
            {
                gasStation.Organizations.Remove(link);
                await _gasStationRepository.UpdateAsync(gasStation);
            }

            return MapGasStationToDto(gasStation);
        }

        private GasStationDto MapGasStationToDto(GasStation gasStation)
        {
            return new GasStationDto
            {
                Id = gasStation.Id,
                CreatedAt = gasStation.CreatedAt,
                UpdatedAt = gasStation.UpdatedAt,
                Name = gasStation.Name,
                Document = gasStation.Document,
                IsGlobal = gasStation.IsGlobal,
                IsActive = gasStation.IsActive,
                Street = gasStation.Street,
                Number = gasStation.Number,
                Neighborhood = gasStation.Neighborhood,
                City = gasStation.City,
                State = gasStation.State,
                ZipCode = gasStation.ZipCode,
                OrganizationIds = gasStation.Organizations.Select(o => o.OrganizationId).ToList()
            };
        }

        private GasStation MapNewDtoToGasStation(NewGasStationDto dto)
        {
            return new GasStation
            {
                Name = dto.Name,
                Document = dto.Document,
                IsGlobal = dto.IsGlobal,
                IsActive = true, // Novos postos são sempre ativos
                Street = dto.Street,
                Number = dto.Number,
                Neighborhood = dto.Neighborhood,
                City = dto.City,
                State = dto.State,
                ZipCode = dto.ZipCode
            };
        }

        private IEnumerable<GasStationDto> MapGasStationListToDtoList(IEnumerable<GasStation> gasStations)
        {
            return gasStations.Select(g => new GasStationDto
            {
                Id = g.Id,
                CreatedAt = g.CreatedAt,
                UpdatedAt = g.UpdatedAt,
                Name = g.Name,
                Document = g.Document,
                IsGlobal = g.IsGlobal,
                IsActive = g.IsActive,
                Street = g.Street,
                Number = g.Number,
                Neighborhood = g.Neighborhood,
                City = g.City,
                State = g.State,
                ZipCode = g.ZipCode,
                OrganizationIds = g.Organizations.Select(o => o.OrganizationId).ToList()
            }).ToList();
        }
    }
}

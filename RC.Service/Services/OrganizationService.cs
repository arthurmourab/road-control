using RC.Domain.Entities;
using RC.Domain.Exceptions;
using RC.Domain.Interfaces.Repositories;
using RC.Domain.Interfaces.Services;
using RC.Shared.Dtos.Organization;
using RC.Shared.Models.Results;

namespace RC.Service.Services
{
    public class OrganizationService(IOrganizationRepository organizationRepository) : IOrganizationService
    {
        private readonly IOrganizationRepository _organizationRepository = organizationRepository;
        public async Task<PagedResult<OrganizationDto>> GetAllAsync(int currentPage, int pageSize)
        {
            var organizations = await _organizationRepository.GetAllAsync(currentPage, pageSize);
            var organizationsTotal = await _organizationRepository.GetAllTotalAsync();

            return new PagedResult<OrganizationDto>
            {
                CurrentPage = currentPage,
                PageSize = pageSize,
                TotalRows = organizationsTotal,
                Results = MapOrganizationListToOrganizationDtoList(organizations)
            };
        }

        public async Task<OrganizationDto> AddAsync(NewOrganizationDto newOrganizationDto)
        {
            var newOrganization = MapNewOrganizationDtoToOrganization(newOrganizationDto);

            var exists = await _organizationRepository.GetByDocumentAsync(newOrganization.Document);
            if (exists != null) throw new ConflictException("Organization already registred.");

            var organization = await _organizationRepository.AddAsync(newOrganization);
            return MapOrganizationToOrganizationDto(organization);
        }

        public async Task<OrganizationDto> GetByIdAsync(long id)
        {
            var organization = await _organizationRepository.GetByIdAsync(id) ?? throw new NotFoundException("Organization not found");
            return MapOrganizationToOrganizationDto(organization);
        }

        private OrganizationDto MapOrganizationToOrganizationDto(Organization organization)
        {
            return new OrganizationDto
            {
                Id = organization.Id,
                CreatedAt = organization.CreatedAt,
                UpdatedAt = organization.UpdatedAt,
                Name = organization.Name,
                Document = organization.Document,
                IsActive = organization.IsActive
            };
        }

        private Organization MapNewOrganizationDtoToOrganization(NewOrganizationDto newOrganizationDto)
        {
            return new Organization
            {
                Name = newOrganizationDto.Name,
                Document = newOrganizationDto.Document,
                IsActive = true, // Novas organizações são sempre ativas
            };
        }

        private IEnumerable<OrganizationDto> MapOrganizationListToOrganizationDtoList(IEnumerable<Organization> organizations)
        {
            return organizations.Select(u => new OrganizationDto
            {
                Id = u.Id,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt,
                Name = u.Name,
                Document = u.Document,
                IsActive = u.IsActive
            }).ToList();
        }
    }
}

using RC.Domain.Entities;
using RC.Domain.Exceptions;
using RC.Domain.Interfaces.Repositories;
using RC.Domain.Interfaces.Services;
using RC.Shared.Dtos.User;
using RC.Shared.Models.Results;

namespace RC.Service.Services
{
    public class UserService(
        IUserRepository userRepository,
        IOrganizationRepository organizationRepository,
        IRoleRepository roleRepository) : IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IOrganizationRepository _organizationRepository = organizationRepository;
        private readonly IRoleRepository _roleRepository = roleRepository;
        public async Task<PagedResult<UserDto>> GetAllAsync(int currentPage, int pageSize,
            long currentUserId, string? currentUserRole, long? organizationId)
        {
            var scopeOrganizationId = await ResolveOrganizationScopeAsync(currentUserId, currentUserRole, organizationId);

            var users = await _userRepository.GetAllAsync(currentPage, pageSize, scopeOrganizationId);
            var userTotal = await _userRepository.GetAllTotalAsync(scopeOrganizationId);

            return new PagedResult<UserDto>
            {
                CurrentPage = currentPage,
                PageSize = pageSize,
                TotalRows = userTotal,
                Results = MapUserListToUserDtoList(users)
            };
        }

        public async Task<UserDto> AddAsync(NewUserDto newUserDto, long currentUserId, string? currentUserRole)
        {
            var role = await _roleRepository.GetByIdAsync(newUserDto.RoleId)
                ?? throw new NotFoundException("Role not found");

            // Resolve a organização conforme quem está cadastrando:
            // OrganizationAdmin usa a própria org; SystemAdmin informa livremente no body
            var organizationId = await ResolveOrganizationIdAsync(newUserDto, currentUserId, currentUserRole);

            // Motoristas precisam pertencer a uma organização
            if (role.Name == Role.Roles.Driver && !organizationId.HasValue)
                throw new BusinessRuleException("Drivers must belong to an organization.");

            if (organizationId.HasValue)
            {
                _ = await _organizationRepository.GetByIdAsync(organizationId.Value)
                    ?? throw new NotFoundException("Organization not found");
            }

            var newUser = MapNewUserDtoToUser(newUserDto, organizationId);

            var exists = await _userRepository.GetByEmailAsync(newUser.Email);
            if (exists != null) throw new ConflictException("User already registred.");

            var user = await _userRepository.AddAsync(newUser);
            return MapUserToUserDto(user);
        }

        private async Task<long?> ResolveOrganizationIdAsync(NewUserDto newUserDto, long currentUserId, string? currentUserRole)
        {
            // SystemAdmin pode cadastrar em qualquer organização (ou nenhuma)
            if (currentUserRole != Role.Roles.OrganizationAdmin)
                return newUserDto.OrganizationId;

            var currentUser = await _userRepository.GetByIdAsync(currentUserId)
                ?? throw new NotFoundException("Current user not found");

            if (!currentUser.OrganizationId.HasValue)
                throw new BusinessRuleException("Organization admin must belong to an organization.");

            // Se informou uma organização diferente da própria, recusa explicitamente
            // (em vez de corrigir em silêncio)
            if (newUserDto.OrganizationId.HasValue && newUserDto.OrganizationId.Value != currentUser.OrganizationId.Value)
                throw new BusinessRuleException("Organization admins can only register users in their own organization.");

            // Omitiu (ou informou a própria): usa a organização do admin
            return currentUser.OrganizationId;
        }

        public async Task<UserDto> GetByIdAsync(long id)
        {
            var user = await _userRepository.GetByIdAsync(id) ?? throw new NotFoundException("User not found");
            return MapUserToUserDto(user);
        }

        public async Task<UserDto> SetActiveAsync(long id, bool isActive, long currentUserId, string? currentUserRole)
        {
            var user = await _userRepository.GetByIdAsync(id) ?? throw new NotFoundException("User not found");

            // OrganizationAdmin só altera usuários da própria organização.
            // Responde 404 (e não 403) para não revelar a existência de usuários de outras organizações.
            var scopeOrganizationId = await ResolveOrganizationScopeAsync(currentUserId, currentUserRole, null);
            if (scopeOrganizationId.HasValue && user.OrganizationId != scopeOrganizationId.Value)
                throw new NotFoundException("User not found");

            user.IsActive = isActive;
            await _userRepository.UpdateAsync(user);

            return MapUserToUserDto(user);
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

        private UserDto MapUserToUserDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                Name = user.Name,
                LastName = user.LastName,
                Email = user.Email,
                IsActive = user.IsActive,
                Role = user.Role.Name,
                OrganizationId = user.OrganizationId
            };
        }

        private User MapNewUserDtoToUser(NewUserDto newUser, long? organizationId)
        {
            return new User
            {
                Name = newUser.Name,
                LastName = newUser.LastName,
                Email = newUser.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(newUser.Password),
                IsActive = true, // Novos usuários são sempre ativos
                RoleId = newUser.RoleId,
                OrganizationId = organizationId, // já resolvida conforme o papel de quem cadastra
            };
        }

        private IEnumerable<UserDto> MapUserListToUserDtoList(IEnumerable<User> users)
        {
            return users.Select(u => new UserDto
            {
                Id = u.Id,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt,
                Name = u.Name,
                LastName = u.LastName,
                Email = u.Email,
                IsActive = u.IsActive,
                Role = u.Role.Name,
                OrganizationId = u.OrganizationId
            }).ToList();
        }
    }
}

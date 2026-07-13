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
        IGasStationRepository gasStationRepository,
        IRoleRepository roleRepository,
        IConfirmationCodeService confirmationCodeService) : IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IOrganizationRepository _organizationRepository = organizationRepository;
        private readonly IGasStationRepository _gasStationRepository = gasStationRepository;
        private readonly IRoleRepository _roleRepository = roleRepository;
        private readonly IConfirmationCodeService _confirmationCodeService = confirmationCodeService;

        public async Task<PagedResult<UserDto>> GetAllAsync(int currentPage, int pageSize,
            long currentUserId, string? currentUserRole, long? organizationId, long? gasStationId)
        {
            var scope = await ResolveCallerScopeAsync(currentUserId, currentUserRole, organizationId, gasStationId);

            var users = await _userRepository.GetAllAsync(currentPage, pageSize, scope.OrganizationId, scope.GasStationId);
            var userTotal = await _userRepository.GetAllTotalAsync(scope.OrganizationId, scope.GasStationId);

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

            // Matriz de alçada: cada papel de chamador só cria papéis da própria alçada
            EnsureCallerCanCreateRole(currentUserRole, role.Name);

            // Resolve os vínculos (organização/posto) conforme quem está cadastrando:
            // OrganizationAdmin usa a própria org, GasStationAdmin usa o próprio posto,
            // SystemAdmin informa livremente no body
            var (organizationId, gasStationId) = await ResolveMembershipAsync(newUserDto, currentUserId, currentUserRole);

            // Coerência papel do novo usuário ↔ vínculo
            ValidateRoleMembership(role.Name, organizationId, gasStationId);

            if (organizationId.HasValue)
            {
                _ = await _organizationRepository.GetByIdAsync(organizationId.Value)
                    ?? throw new NotFoundException("Organization not found");
            }

            if (gasStationId.HasValue)
            {
                _ = await _gasStationRepository.GetByIdAsync(gasStationId.Value)
                    ?? throw new NotFoundException("Gas station not found");
            }

            var newUser = MapNewUserDtoToUser(newUserDto, organizationId, gasStationId);

            // Frentista já nasce com segredo de código de confirmação provisionado
            if (role.Name == Role.Roles.GasStationAttendant)
                newUser.ConfirmationSecret = _confirmationCodeService.GenerateSecret();

            var exists = await _userRepository.GetByEmailAsync(newUser.Email);
            if (exists != null) throw new ConflictException("User already registred.");

            var user = await _userRepository.AddAsync(newUser);
            return MapUserToUserDto(user);
        }

        // Papéis que cada chamador pode criar:
        // SystemAdmin → qualquer; OrganizationAdmin → OrganizationAdmin e Driver;
        // GasStationAdmin → GasStationAdmin e GasStationAttendant.
        private static void EnsureCallerCanCreateRole(string? currentUserRole, string targetRoleName)
        {
            switch (currentUserRole)
            {
                case Role.Roles.SystemAdmin:
                    return;

                case Role.Roles.OrganizationAdmin:
                    if (targetRoleName != Role.Roles.OrganizationAdmin && targetRoleName != Role.Roles.Driver)
                        throw new BusinessRuleException("Organization admins can only create organization admins and drivers.");
                    return;

                case Role.Roles.GasStationAdmin:
                    if (targetRoleName != Role.Roles.GasStationAdmin && targetRoleName != Role.Roles.GasStationAttendant)
                        throw new BusinessRuleException("Gas station admins can only create gas station admins and gas station attendants.");
                    return;

                default:
                    // O [Authorize] do controller já restringe a UserManagers — defesa em profundidade
                    throw new BusinessRuleException("Caller role is not allowed to create users.");
            }
        }

        private async Task<(long? OrganizationId, long? GasStationId)> ResolveMembershipAsync(
            NewUserDto newUserDto, long currentUserId, string? currentUserRole)
        {
            // SystemAdmin informa os vínculos livremente no body
            if (currentUserRole == Role.Roles.SystemAdmin)
                return (newUserDto.OrganizationId, newUserDto.GasStationId);

            var currentUser = await _userRepository.GetByIdAsync(currentUserId)
                ?? throw new NotFoundException("Current user not found");

            if (currentUserRole == Role.Roles.OrganizationAdmin)
            {
                if (!currentUser.OrganizationId.HasValue)
                    throw new BusinessRuleException("Organization admin must belong to an organization.");

                if (newUserDto.GasStationId.HasValue)
                    throw new BusinessRuleException("Organization admins cannot assign users to a gas station.");

                // Se informou uma organização diferente da própria, recusa explicitamente
                // (em vez de corrigir em silêncio)
                if (newUserDto.OrganizationId.HasValue && newUserDto.OrganizationId.Value != currentUser.OrganizationId.Value)
                    throw new BusinessRuleException("Organization admins can only register users in their own organization.");

                // Omitiu (ou informou a própria): usa a organização do admin
                return (currentUser.OrganizationId, null);
            }

            // GasStationAdmin — espelho da regra do OrganizationAdmin, para postos
            if (!currentUser.GasStationId.HasValue)
                throw new BusinessRuleException("Gas station admin must belong to a gas station.");

            if (newUserDto.OrganizationId.HasValue)
                throw new BusinessRuleException("Gas station admins cannot assign users to an organization.");

            if (newUserDto.GasStationId.HasValue && newUserDto.GasStationId.Value != currentUser.GasStationId.Value)
                throw new BusinessRuleException("Gas station admins can only register users in their own gas station.");

            return (null, currentUser.GasStationId);
        }

        // Coerência entre o papel do novo usuário e seus vínculos:
        // papéis de organização exigem organização; papéis de posto exigem posto;
        // SystemAdmin não tem vínculo algum; ninguém tem os dois ao mesmo tempo.
        private static void ValidateRoleMembership(string roleName, long? organizationId, long? gasStationId)
        {
            if (organizationId.HasValue && gasStationId.HasValue)
                throw new BusinessRuleException("A user cannot belong to an organization and a gas station at the same time.");

            switch (roleName)
            {
                case Role.Roles.SystemAdmin:
                    if (organizationId.HasValue || gasStationId.HasValue)
                        throw new BusinessRuleException("System admins cannot belong to an organization or a gas station.");
                    break;

                case Role.Roles.OrganizationAdmin:
                case Role.Roles.Driver:
                    if (!organizationId.HasValue)
                        throw new BusinessRuleException("Organization admins and drivers must belong to an organization.");
                    break;

                case Role.Roles.GasStationAdmin:
                case Role.Roles.GasStationAttendant:
                    if (!gasStationId.HasValue)
                        throw new BusinessRuleException("Gas station admins and attendants must belong to a gas station.");
                    break;
            }
        }

        public async Task<UserDto> GetByIdAsync(long id)
        {
            var user = await _userRepository.GetByIdAsync(id) ?? throw new NotFoundException("User not found");
            return MapUserToUserDto(user);
        }

        public async Task<UserDto> SetActiveAsync(long id, bool isActive, long currentUserId, string? currentUserRole)
        {
            var user = await _userRepository.GetByIdAsync(id) ?? throw new NotFoundException("User not found");

            // Chamador não-SystemAdmin só altera usuários do próprio escopo (organização ou posto).
            // Responde 404 (e não 403) para não revelar a existência de usuários fora do escopo.
            var scope = await ResolveCallerScopeAsync(currentUserId, currentUserRole, null, null);
            if (scope.OrganizationId.HasValue && user.OrganizationId != scope.OrganizationId.Value)
                throw new NotFoundException("User not found");
            if (scope.GasStationId.HasValue && user.GasStationId != scope.GasStationId.Value)
                throw new NotFoundException("User not found");

            // OrganizationAdmin só altera o status de motoristas e GasStationAdmin só o de
            // frentistas (nunca de outros admins nem o próprio). Aqui o alvo já é
            // comprovadamente do mesmo escopo, então 422 não revela nada novo.
            if (currentUserRole == Role.Roles.OrganizationAdmin && user.Role.Name != Role.Roles.Driver)
                throw new BusinessRuleException("Organization admins can only change the status of drivers.");
            if (currentUserRole == Role.Roles.GasStationAdmin && user.Role.Name != Role.Roles.GasStationAttendant)
                throw new BusinessRuleException("Gas station admins can only change the status of gas station attendants.");

            user.IsActive = isActive;
            await _userRepository.UpdateAsync(user);

            return MapUserToUserDto(user);
        }

        // Resolve o escopo do chamador (organização OU posto — nunca ambos):
        // SystemAdmin enxerga tudo (podendo filtrar por organização e/ou posto específicos);
        // os demais papéis ficam restritos à própria organização ou ao próprio posto.
        private async Task<(long? OrganizationId, long? GasStationId)> ResolveCallerScopeAsync(
            long currentUserId, string? currentUserRole, long? requestedOrganizationId, long? requestedGasStationId)
        {
            if (currentUserRole == Role.Roles.SystemAdmin)
                return (requestedOrganizationId, requestedGasStationId);

            var currentUser = await _userRepository.GetByIdAsync(currentUserId)
                ?? throw new NotFoundException("Current user not found");

            if (currentUser.OrganizationId.HasValue)
                return (currentUser.OrganizationId, null);

            if (currentUser.GasStationId.HasValue)
                return (null, currentUser.GasStationId);

            throw new BusinessRuleException("User must belong to an organization or a gas station.");
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
                OrganizationId = user.OrganizationId,
                OrganizationName = user.Organization?.Name,
                GasStationId = user.GasStationId,
                GasStationName = user.GasStation?.Name
            };
        }

        private User MapNewUserDtoToUser(NewUserDto newUser, long? organizationId, long? gasStationId)
        {
            return new User
            {
                Name = newUser.Name,
                LastName = newUser.LastName,
                Email = newUser.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(newUser.Password),
                IsActive = true, // Novos usuários são sempre ativos
                RoleId = newUser.RoleId,
                OrganizationId = organizationId, // vínculos já resolvidos conforme o papel de quem cadastra
                GasStationId = gasStationId
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
                OrganizationId = u.OrganizationId,
                OrganizationName = u.Organization?.Name,
                GasStationId = u.GasStationId,
                GasStationName = u.GasStation?.Name
            }).ToList();
        }
    }
}

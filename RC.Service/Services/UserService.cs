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
        IOrganizationRepository organizationRepository) : IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IOrganizationRepository _organizationRepository = organizationRepository;
        public async Task<PagedResult<UserDto>> GetAllAsync(int currentPage, int pageSize)
        {
            var users = await _userRepository.GetAllAsync(currentPage, pageSize);
            var userTotal = await _userRepository.GetAllTotalAsync();

            return new PagedResult<UserDto>
            {
                CurrentPage = currentPage,
                PageSize = pageSize,
                TotalRows = userTotal,
                Results = MapUserListToUserDtoList(users)
            };
        }

        public async Task<UserDto> AddAsync(NewUserDto newUserDto)
        {
            if (newUserDto.OrganizationId.HasValue)
            {
                _ = await _organizationRepository.GetByIdAsync(newUserDto.OrganizationId.Value)
                    ?? throw new NotFoundException("Organization not found");
            }

            var newUser = MapNewUserDtoToUser(newUserDto);

            var exists = await _userRepository.GetByEmailAsync(newUser.Email);
            if (exists != null) throw new ConflictException("User already registred.");

            var user = await _userRepository.AddAsync(newUser);
            return MapUserToUserDto(user);
        }

        public async Task<UserDto> GetByIdAsync(long id)
        {
            var user = await _userRepository.GetByIdAsync(id) ?? throw new NotFoundException("User not found");
            return MapUserToUserDto(user);
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

        private User MapNewUserDtoToUser(NewUserDto newUser)
        {
            return new User
            {
                Name = newUser.Name,
                LastName = newUser.LastName,
                Email = newUser.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(newUser.Password),
                IsActive = true, // Novos usuários são sempre ativos
                RoleId = newUser.RoleId,
                OrganizationId = newUser.OrganizationId,
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

using RC.Domain.Entities;
using RC.Domain.Interfaces.Repositories;
using RC.Domain.Interfaces.Services;
using RC.Shared.Dtos.User;
using RC.Shared.Models.Results;

namespace RC.Service.Services
{
    public class UserService(IUserRepository userRepository) : IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;
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

        public async Task<UserDto> AddAsync(NewUserDto newUser)
        {
            var newUserEntity = MapNewUserDtoToUser(newUser);
            var userEntity = await _userRepository.AddAsync(newUserEntity);
            return MapUserToUserDto(userEntity);
        }

        public async Task<UserDto?> GetByIdAsync(long id)
        {
            var user = await _userRepository.GetByIdAsync(id);
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
                Role = user.Role.Name
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
                Role = u.Role.Name
            }).ToList();
        }
    }
}

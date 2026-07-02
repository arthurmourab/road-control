using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RC.Domain.Entities;
using RC.Domain.Exceptions;
using RC.Domain.Interfaces.Repositories;
using RC.Domain.Interfaces.Services;
using RC.Shared.Dtos.Authentication;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RC.Service.Services
{
    public class AuthService(IUserRepository userRepository, IConfiguration configuration) : IAuthService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IConfiguration _configuration = configuration;

        // TODO: substituir por fluxo de e-mail — reset provisório define a senha como "admin"
        // enquanto não existe o serviço de envio de e-mail com senha/link de redefinição.
        private const string TemporaryPassword = "admin";

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequest)
        {
            var user = await _userRepository.GetByEmailAsync(loginRequest.Email);

            // Usuário inexistente, inativo ou senha incorreta: mesma resposta genérica,
            // para não revelar a um atacante se a conta existe
            if (user is null || !user.IsActive || !BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.PasswordHash))
                throw new UnauthorizedAccessException();

            var jwtConfig = _configuration.GetSection("Jwt");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig["Secret"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiresAt = DateTime.UtcNow.AddMinutes(int.Parse(jwtConfig["ExpirationMinutes"]!));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.Name),
                new Claim(JwtRegisteredClaimNames.GivenName, user.Name),
                new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtConfig["Issuer"],
                audience: jwtConfig["Audience"],
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials
            );

            return new LoginResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiresAt = expiresAt,
                Name = user.Name,
                LastName = user.LastName,
                Role = user.Role.Name
            };
        }

        public async Task ResetPasswordAsync(ResetPasswordDto resetPassword)
        {
            var user = await _userRepository.GetByEmailAsync(resetPassword.Email);

            // E-mail inexistente: retorna silenciosamente — a resposta é sempre a mesma
            // genérica no controller, para não revelar quais e-mails têm conta.
            // Usuário inativo também é resetado sem efeito prático: o login bloqueia inativos.
            if (user is null)
                return;

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(TemporaryPassword);
            await _userRepository.UpdateAsync(user);
        }

        // Troca de senha do próprio usuário autenticado (tela de perfil):
        // exige a senha atual correta antes de gravar a nova.
        public async Task ChangePasswordAsync(ChangePasswordDto changePassword, long currentUserId)
        {
            var user = await _userRepository.GetByIdAsync(currentUserId)
                ?? throw new NotFoundException("User not found");

            // Senha atual incorreta é regra de negócio (422), não 401 — um 401 aqui
            // faria o interceptor do frontend derrubar a sessão do usuário logado.
            if (!BCrypt.Net.BCrypt.Verify(changePassword.CurrentPassword, user.PasswordHash))
                throw new BusinessRuleException("Current password is incorrect.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(changePassword.NewPassword);
            await _userRepository.UpdateAsync(user);
        }
    }
}

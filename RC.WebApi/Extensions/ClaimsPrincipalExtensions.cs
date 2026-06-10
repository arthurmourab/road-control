using System.Security.Claims;

namespace RC.WebApi.Extensions
{
    // Helpers para extrair dados do usuário autenticado a partir dos claims do JWT
    public static class ClaimsPrincipalExtensions
    {
        // Extrai o id do usuário autenticado (claim 'sub' / NameIdentifier)
        public static long GetUserId(this ClaimsPrincipal user)
        {
            var sub = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue("sub");
            if (!long.TryParse(sub, out var userId))
                throw new UnauthorizedAccessException("Invalid token: missing user id.");

            return userId;
        }

        // Extrai o papel (role) do usuário autenticado
        public static string? GetRole(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.Role) ?? user.FindFirstValue("role");
        }
    }
}

using System.ComponentModel.DataAnnotations;

namespace RC.Shared.Dtos.Authentication
{
    public class ChangePasswordDto
    {
        [Required]
        public string CurrentPassword { get; set; } = string.Empty;

        // Mesmas regras de senha do cadastro de usuário (NewUserDto)
        [Required]
        [MinLength(8)]
        public string NewPassword { get; set; } = string.Empty;
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RC.Shared.Dtos.User
{
    public class NewUserDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; }

        [Required]
        [MinLength(8)]
        public string Password { get; set; }

        [Range(1, long.MaxValue)]
        public long RoleId { get; set; }

        // Opcional — SystemAdmin não pertence a nenhuma organização
        public long? OrganizationId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RC.Shared.Dtos.User
{
    public class NewUserDto
    {
        public  string Name { get; set; }
        public string LastName { get; set; }

        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
        public long RoleId { get; set; }

        // Opcional — SystemAdmin não pertence a nenhuma organização
        public long? OrganizationId { get; set; }
    }
}

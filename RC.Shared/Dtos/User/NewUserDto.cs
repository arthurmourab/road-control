using System;
using System.Collections.Generic;
using System.Text;

namespace RC.Shared.Dtos.User
{
    public class NewUserDto
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public long RoleId { get; set; }
    }
}

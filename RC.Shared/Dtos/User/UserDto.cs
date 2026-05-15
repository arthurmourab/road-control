using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace RC.Shared.Dtos.User
{
    public class UserDto
    {
        public long Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public string Role { get; set; }
    }
}

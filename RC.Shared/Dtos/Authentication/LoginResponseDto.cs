using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

namespace RC.Shared.Dtos.Authentication
{
    public class LoginResponseDto
    {
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace RC.Shared.Dtos.Authentication
{
    public class LoginRequestDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}

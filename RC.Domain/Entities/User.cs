using System;
using System.Collections.Generic;
using System.Text;

namespace RC.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public bool IsActive { get; set; }
        public long RoleId { get; set; }
        public Role Role { get; set; }
    }
}

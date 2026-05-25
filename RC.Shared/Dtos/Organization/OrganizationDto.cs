using System;
using System.Collections.Generic;
using System.Text;

namespace RC.Shared.Dtos.Organization
{
    public class OrganizationDto
    {
        public long Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string Name { get; set; }
        public string Document { get; set; }
        public bool IsActive { get; set; }
    }
}

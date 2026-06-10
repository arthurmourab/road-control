using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RC.Shared.Dtos.Organization
{
    public class NewOrganizationDto
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        [MaxLength(14)]
        public string Document { get; set; }
    }
}

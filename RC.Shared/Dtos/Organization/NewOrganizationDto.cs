using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RC.Shared.Dtos.Organization
{
    public class NewOrganizationDto
    {
        public string Name { get; set; }

        [MaxLength(14)]
        public string Document { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace RC.Shared.Dtos.GasStation
{
    public class LinkOrganizationsDto
    {
        [Required]
        [MinLength(1)]
        public IEnumerable<long> OrganizationIds { get; set; }
    }
}

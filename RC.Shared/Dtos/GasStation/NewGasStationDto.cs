using System.ComponentModel.DataAnnotations;

namespace RC.Shared.Dtos.GasStation
{
    public class NewGasStationDto
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        [MaxLength(14)]
        public string Document { get; set; }

        public bool IsGlobal { get; set; }

        [Required]
        [MaxLength(255)]
        public string Street { get; set; }

        [Required]
        [MaxLength(20)]
        public string Number { get; set; }

        [Required]
        [MaxLength(100)]
        public string Neighborhood { get; set; }

        [Required]
        [MaxLength(100)]
        public string City { get; set; }

        [Required]
        [MaxLength(2)]
        public string State { get; set; }

        [Required]
        [MaxLength(8)]
        public string ZipCode { get; set; }
    }
}

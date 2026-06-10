using System.ComponentModel.DataAnnotations;

namespace RC.Shared.Dtos.GasStation
{
    // O Document (CNPJ) não é editável — identifica o posto desde o cadastro
    public class UpdateGasStationDto
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        public bool IsGlobal { get; set; }
        public bool IsActive { get; set; }

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

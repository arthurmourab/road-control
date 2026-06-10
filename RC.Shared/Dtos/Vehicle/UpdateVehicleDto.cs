using RC.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace RC.Shared.Dtos.Vehicle
{
    public class UpdateVehicleDto
    {
        public VehicleTypeEnum Type { get; set; }

        [Required]
        [MaxLength(10)]
        public string Plate { get; set; }

        [Required]
        [MaxLength(100)]
        public string Brand { get; set; }

        [Required]
        [MaxLength(100)]
        public string Model { get; set; }

        [Range(1900, 2100)]
        public int YearManufacture { get; set; }

        [Range(1900, 2100)]
        public int YearModel { get; set; }

        [Range(0, int.MaxValue)]
        public int Mileage { get; set; }
    }
}

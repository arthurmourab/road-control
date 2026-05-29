using RC.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace RC.Shared.Dtos.Vehicle
{
    public class NewVehicleDto
    {
        public VehicleTypeEnum Type { get; set; }
        public string Plate { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int YearManufacture { get; set; }
        public int YearModel { get; set; }
        public int Mileage { get; set; }

        [Range(1, long.MaxValue)]
        public long OrganizationId { get; set; }
    }
}

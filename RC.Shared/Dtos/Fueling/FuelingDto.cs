using RC.Shared.Enums;

namespace RC.Shared.Dtos.Fueling
{
    public class FuelingDto
    {
        public long Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public long VehicleId { get; set; }
        public long GasStationId { get; set; }
        public long DriverId { get; set; }
        public long OrganizationId { get; set; }
        public FuelTypeEnum FuelType { get; set; }
        public decimal Liters { get; set; }
        public decimal PricePerLiter { get; set; }
        public decimal TotalAmount { get; set; }
        public int Mileage { get; set; }
        public DateTime FueledAt { get; set; }
    }
}

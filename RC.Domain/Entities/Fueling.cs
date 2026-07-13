using RC.Shared.Enums;

namespace RC.Domain.Entities
{
    public class Fueling : BaseEntity
    {
        public long VehicleId { get; set; }
        public Vehicle Vehicle { get; set; }

        public long GasStationId { get; set; }
        public GasStation GasStation { get; set; }

        // Motorista que registrou o abastecimento
        public long DriverId { get; set; }
        public User Driver { get; set; }

        // Frentista que forneceu o código de confirmação.
        // Nullable no banco (registros históricos não têm), mas SEMPRE preenchido em novos.
        public long? AttendantId { get; set; }
        public User? Attendant { get; set; }

        // Organização dona do veículo no momento do abastecimento (snapshot)
        public long OrganizationId { get; set; }
        public Organization Organization { get; set; }

        public FuelTypeEnum FuelType { get; set; }
        public decimal Liters { get; set; }
        public decimal PricePerLiter { get; set; }
        public decimal TotalAmount { get; set; }
        public int Mileage { get; set; }
        public DateTime FueledAt { get; set; }
    }
}

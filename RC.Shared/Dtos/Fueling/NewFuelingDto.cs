using RC.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace RC.Shared.Dtos.Fueling
{
    public class NewFuelingDto
    {
        [Range(1, long.MaxValue)]
        public long VehicleId { get; set; }

        [Range(1, long.MaxValue)]
        public long GasStationId { get; set; }

        // Obrigatório apenas quando quem registra é OrganizationAdmin/SystemAdmin.
        // O Driver usa o próprio id do token. Validado no service.
        public long? DriverId { get; set; }

        public FuelTypeEnum FuelType { get; set; }

        public decimal Liters { get; set; }
        public decimal PricePerLiter { get; set; }
        public int Mileage { get; set; }
        public DateTime FueledAt { get; set; }
    }
}

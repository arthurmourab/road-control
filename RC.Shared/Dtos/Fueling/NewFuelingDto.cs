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

        [Range(0.001, 99999.999)]
        public decimal Liters { get; set; }

        [Range(0.001, 999999.999)]
        public decimal PricePerLiter { get; set; }

        [Range(0, int.MaxValue)]
        public int Mileage { get; set; }

        public DateTime FueledAt { get; set; }

        // Código de confirmação fornecido presencialmente pelo frentista. Obrigatório
        // em todo registro (inclusive quando gestor/admin lança em nome do motorista).
        [Required]
        public string ConfirmationCode { get; set; } = string.Empty;
    }
}

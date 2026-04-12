using RC.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace RC.Shared.Dtos
{
    public class VehicleDto
    {
        public long? Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public VehicleTypeEnum Type { get; set; }
        public string Plate { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int YearManufacture { get; set; }
        public int YearModel { get; set; }
        public int Mileage { get; set; }
        public bool? IsActive { get; set; }
    }
}

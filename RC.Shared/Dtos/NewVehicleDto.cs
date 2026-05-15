using RC.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace RC.Shared.Dtos
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
    }
}

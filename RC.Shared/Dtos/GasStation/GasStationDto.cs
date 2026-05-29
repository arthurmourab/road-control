namespace RC.Shared.Dtos.GasStation
{
    public class GasStationDto
    {
        public long Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string Name { get; set; }
        public string Document { get; set; }
        public bool IsGlobal { get; set; }
        public bool IsActive { get; set; }
        public string Street { get; set; }
        public string Number { get; set; }
        public string Neighborhood { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }

        // Ids das organizações vinculadas (vazio quando o posto é global)
        public IEnumerable<long> OrganizationIds { get; set; } = new List<long>();
    }
}

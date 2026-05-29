namespace RC.Domain.Entities
{
    public class GasStation : BaseEntity
    {
        public string Name { get; set; }
        public string Document { get; set; }

        // Quando true, o posto é parceiro global e está disponível a todas as organizações.
        // Quando false, fica disponível apenas às organizações vinculadas (Organizations).
        public bool IsGlobal { get; set; }
        public bool IsActive { get; set; }

        // Endereço
        public string Street { get; set; }
        public string Number { get; set; }
        public string Neighborhood { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }

        // Organizações vinculadas (relevante apenas quando IsGlobal = false)
        public ICollection<OrganizationGasStation> Organizations { get; set; } = new List<OrganizationGasStation>();
    }
}

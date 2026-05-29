namespace RC.Domain.Entities
{
    // Tabela de associação N:N entre organizações e postos não-globais.
    public class OrganizationGasStation : BaseEntity
    {
        public long OrganizationId { get; set; }
        public Organization Organization { get; set; }

        public long GasStationId { get; set; }
        public GasStation GasStation { get; set; }
    }
}

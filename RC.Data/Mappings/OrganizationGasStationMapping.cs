using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RC.Domain.Entities;

namespace RC.Data.Mappings
{
    public class OrganizationGasStationMapping : IEntityTypeConfiguration<OrganizationGasStation>
    {
        public void Configure(EntityTypeBuilder<OrganizationGasStation> builder)
        {
            builder.ToTable("OrganizationGasStations", "rc");

            builder.HasKey(o => o.Id);

            builder.Property(o => o.Id).ValueGeneratedOnAdd();
            builder.Property(o => o.OrganizationId).HasColumnName("OrganizationId").IsRequired();
            builder.Property(o => o.GasStationId).HasColumnName("GasStationId").IsRequired();
            builder.Property(o => o.CreatedAt).HasColumnName("CreatedAt").IsRequired();
            builder.Property(o => o.UpdatedAt).HasColumnName("UpdatedAt");

            builder.HasOne(o => o.Organization)
                .WithMany()
                .HasForeignKey(o => o.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(o => o.GasStation)
                .WithMany(g => g.Organizations)
                .HasForeignKey(o => o.GasStationId)
                .OnDelete(DeleteBehavior.Restrict);

            // Impede vincular a mesma organização ao mesmo posto mais de uma vez
            builder.HasIndex(o => new { o.OrganizationId, o.GasStationId }).IsUnique();
        }
    }
}

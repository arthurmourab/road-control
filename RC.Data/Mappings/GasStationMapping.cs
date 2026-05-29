using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RC.Domain.Entities;

namespace RC.Data.Mappings
{
    public class GasStationMapping : IEntityTypeConfiguration<GasStation>
    {
        public void Configure(EntityTypeBuilder<GasStation> builder)
        {
            builder.ToTable("GasStations", "rc");

            builder.HasKey(g => g.Id);

            builder.Property(g => g.Id).ValueGeneratedOnAdd();
            builder.Property(g => g.Name).HasColumnName("Name").HasMaxLength(255).IsRequired();
            builder.Property(g => g.Document).HasColumnName("Document").HasMaxLength(14).IsRequired();
            builder.Property(g => g.IsGlobal).HasColumnName("IsGlobal").IsRequired();
            builder.Property(g => g.IsActive).HasColumnName("IsActive").IsRequired();
            builder.Property(g => g.Street).HasColumnName("Street").HasMaxLength(255).IsRequired();
            builder.Property(g => g.Number).HasColumnName("Number").HasMaxLength(20).IsRequired();
            builder.Property(g => g.Neighborhood).HasColumnName("Neighborhood").HasMaxLength(100).IsRequired();
            builder.Property(g => g.City).HasColumnName("City").HasMaxLength(100).IsRequired();
            builder.Property(g => g.State).HasColumnName("State").HasMaxLength(2).IsRequired();
            builder.Property(g => g.ZipCode).HasColumnName("ZipCode").HasMaxLength(8).IsRequired();
            builder.Property(g => g.CreatedAt).HasColumnName("CreatedAt").IsRequired();
            builder.Property(g => g.UpdatedAt).HasColumnName("UpdatedAt");

            builder.HasIndex(g => g.Document).IsUnique();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RC.Domain.Entities;

namespace RC.Data.Mappings
{
    public class FuelingMapping : IEntityTypeConfiguration<Fueling>
    {
        public void Configure(EntityTypeBuilder<Fueling> builder)
        {
            builder.ToTable("Fuelings", "rc");

            builder.HasKey(f => f.Id);

            builder.Property(f => f.Id).ValueGeneratedOnAdd();
            builder.Property(f => f.VehicleId).HasColumnName("VehicleId").IsRequired();
            builder.Property(f => f.GasStationId).HasColumnName("GasStationId").IsRequired();
            builder.Property(f => f.DriverId).HasColumnName("DriverId").IsRequired();
            builder.Property(f => f.AttendantId).HasColumnName("AttendantId");
            builder.Property(f => f.OrganizationId).HasColumnName("OrganizationId").IsRequired();
            builder.Property(f => f.FuelType).HasColumnName("FuelType").HasConversion<int>().IsRequired();
            builder.Property(f => f.Liters).HasColumnName("Liters").HasColumnType("decimal(9,3)").IsRequired();
            builder.Property(f => f.PricePerLiter).HasColumnName("PricePerLiter").HasColumnType("decimal(18,3)").IsRequired();
            builder.Property(f => f.TotalAmount).HasColumnName("TotalAmount").HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(f => f.Mileage).HasColumnName("Mileage").IsRequired();
            builder.Property(f => f.FueledAt).HasColumnName("FueledAt").IsRequired();
            builder.Property(f => f.CreatedAt).HasColumnName("CreatedAt").IsRequired();
            builder.Property(f => f.UpdatedAt).HasColumnName("UpdatedAt");

            builder.HasOne(f => f.Vehicle)
                .WithMany()
                .HasForeignKey(f => f.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(f => f.GasStation)
                .WithMany()
                .HasForeignKey(f => f.GasStationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(f => f.Driver)
                .WithMany()
                .HasForeignKey(f => f.DriverId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(f => f.Attendant)
                .WithMany()
                .HasForeignKey(f => f.AttendantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(f => f.Organization)
                .WithMany()
                .HasForeignKey(f => f.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

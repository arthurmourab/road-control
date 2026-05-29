using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RC.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RC.Data.Mappings
{
    public class VehicleMapping : IEntityTypeConfiguration<Vehicle>
    {
        public void Configure(EntityTypeBuilder<Vehicle> builder)
        {
            builder.ToTable("Vehicles", "rc");

            builder.HasKey(v => v.Id);

            builder.Property(v => v.Id).ValueGeneratedOnAdd();
            builder.Property(v => v.Type).HasColumnName("Type").HasConversion<int>().IsRequired();
            builder.Property(v => v.Plate).HasColumnName("Plate").HasMaxLength(10).IsRequired();
            builder.Property(v => v.Brand).HasColumnName("Brand").HasMaxLength(100).IsRequired();
            builder.Property(v => v.Model).HasColumnName("Model").HasMaxLength(100).IsRequired();
            builder.Property(v => v.YearManufacture).HasColumnName("YearManufacture").IsRequired();
            builder.Property(v => v.YearModel).HasColumnName("YearModel").IsRequired();
            builder.Property(v => v.Mileage).HasColumnName("Mileage").IsRequired();
            builder.Property(v => v.IsActive).HasColumnName("IsActive").IsRequired();
            builder.Property(v => v.CreatedAt).HasColumnName("CreatedAt").IsRequired();
            builder.Property(v => v.UpdatedAt).HasColumnName("UpdatedAt");
            builder.Property(v => v.OrganizationId).HasColumnName("OrganizationId").IsRequired();

            builder.HasIndex(v => v.Plate).IsUnique();

            builder.HasOne(v => v.Organization)
                .WithMany()
                .HasForeignKey(v => v.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

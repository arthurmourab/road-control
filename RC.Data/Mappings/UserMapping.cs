using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RC.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RC.Data.Mappings
{
    public class UserMapping : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users", "rc");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Id).ValueGeneratedOnAdd();
            builder.Property(u => u.Name).HasColumnName("Name").HasMaxLength(100).IsRequired();
            builder.Property(u => u.LastName).HasColumnName("LastName").HasMaxLength(100).IsRequired();
            builder.Property(u => u.Email).HasColumnName("Email").HasMaxLength(255).IsRequired();
            builder.Property(u => u.PasswordHash).HasColumnName("PasswordHash").HasMaxLength(500).IsRequired();
            builder.Property(u => u.RoleId).HasColumnName("RoleId").IsRequired();
            builder.Property(u => u.IsActive).HasColumnName("IsActive").IsRequired();
            builder.Property(u => u.CreatedAt).HasColumnName("CreatedAt").IsRequired();
            builder.Property(u => u.UpdatedAt).HasColumnName("UpdatedAt");
            builder.Property(u => u.OrganizationId).HasColumnName("OrganizationId");
            builder.Property(u => u.GasStationId).HasColumnName("GasStationId");

            builder.HasIndex(u => u.Email).IsUnique();

            builder.HasOne(u => u.Role)
                .WithMany()
                .HasForeignKey(u => u.RoleId);

            builder.HasOne(u => u.Organization)
                .WithMany()
                .HasForeignKey(u => u.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(u => u.GasStation)
                .WithMany()
                .HasForeignKey(u => u.GasStationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

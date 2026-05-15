using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RC.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RC.Data.Mappings
{
    public class RoleMapping : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Roles", "rc");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Id).ValueGeneratedOnAdd();
            builder.Property(r => r.Name).HasColumnName("Name").IsRequired();
            builder.Property(r => r.Description).HasColumnName("Description").IsRequired();
            builder.Property(r => r.CreatedAt).HasColumnName("CreatedAt").IsRequired();
            builder.Property(r => r.UpdatedAt).HasColumnName("UpdatedAt");
        }
    }
}

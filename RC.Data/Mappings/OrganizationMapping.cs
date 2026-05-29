using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RC.Domain.Entities;

namespace RC.Data.Mappings
{
    public class OrganizationMapping : IEntityTypeConfiguration<Organization>
    {
        public void Configure(EntityTypeBuilder<Organization> builder)
        {
            builder.ToTable("Organizations", "rc");

            builder.HasKey(o => o.Id);

            builder.Property(o => o.Id).ValueGeneratedOnAdd();
            builder.Property(o => o.Name).HasColumnName("Name").HasMaxLength(255).IsRequired();
            builder.Property(o => o.Document).HasColumnName("Document").HasMaxLength(14).IsRequired();
            builder.Property(o => o.IsActive).HasColumnName("IsActive").IsRequired();
            builder.Property(o => o.CreatedAt).HasColumnName("CreatedAt").IsRequired();
            builder.Property(o => o.UpdatedAt).HasColumnName("UpdatedAt");

            builder.HasIndex(o => o.Document).IsUnique();
        }
    }
}

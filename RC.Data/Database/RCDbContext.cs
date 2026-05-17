using Microsoft.EntityFrameworkCore;
using RC.Data.Mappings;
using RC.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RC.Data.Database
{
    public class RCDbContext(DbContextOptions<RCDbContext> options) : DbContext(options)
    {

        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(VehicleMapping).Assembly);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Added)
                    entry.Property(x => x.CreatedAt).CurrentValue = DateTime.UtcNow;

                if (entry.State is EntityState.Added or EntityState.Modified)
                    entry.Property(x => x.UpdatedAt).CurrentValue = DateTime.UtcNow;
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}

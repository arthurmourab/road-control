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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(VehicleMapping).Assembly);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }
    }
}

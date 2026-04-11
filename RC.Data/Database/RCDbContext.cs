using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace RC.Data.Database
{
    public class RCDbContext(DbContextOptions<RCDbContext> options) : DbContext()
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }
    }
}

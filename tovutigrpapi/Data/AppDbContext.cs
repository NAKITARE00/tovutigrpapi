using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using tovutigrpapi.Models;

namespace tovutigrpapi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Stations> Tovutis { get; set; }  
        public DbSet<Users> Users { get; set; }
    }
}


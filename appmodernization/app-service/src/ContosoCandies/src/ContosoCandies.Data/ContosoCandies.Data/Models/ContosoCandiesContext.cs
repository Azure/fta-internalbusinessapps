using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ContosoCandies.Data.Models
{
    public class ContosoCandiesContext : DbContext
    {
        
        public ContosoCandiesContext(DbContextOptions options) : base(options)
        {
        }

        public ContosoCandiesContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }


        public virtual DbSet<Candies> Candies { get; set; }
        public virtual DbSet<OrderLines> OrderLines { get; set; }
        public virtual DbSet<Orders> Orders { get; set; }
        public virtual DbSet<Stores> Stores { get; set; }
    }
}
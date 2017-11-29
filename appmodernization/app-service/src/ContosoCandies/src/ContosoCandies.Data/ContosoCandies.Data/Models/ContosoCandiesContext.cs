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

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    #warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.

        //    optionsBuilder.UseSqlServer(@"Server=UMARM-PC15\UMARSQLSERVER;Database=ContosoCandies;Trusted_Connection=True;");

        // //   optionsBuilder.UseSqlServer(@"Server=tcp:contosocandies-sqlsrv.database.windows.net,1433;Initial Catalog=contosocandies;Persist Security Info=False;User ID=umarm;Password=ContosoCandiesStrongPassword123!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
        //}

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Candies>(entity =>
        //    {
        //        entity.Property(e => e.ImageUrl).HasMaxLength(256);

        //        entity.Property(e => e.Name).HasColumnType("varchar(50)");
        //    });

        //    modelBuilder.Entity<OrderLines>(entity =>
        //    {
        //        entity.HasOne(d => d.Candie)
        //            .WithMany(p => p.OrderLines)
        //            .HasForeignKey(d => d.CandieId)
        //            .HasConstraintName("FK_OrderLines_Candies");

        //        entity.HasOne(d => d.Order)
        //            .WithMany(p => p.OrderLines)
        //            .HasForeignKey(d => d.OrderId)
        //            .OnDelete(DeleteBehavior.Cascade)
        //            .HasConstraintName("FK_OrderLines_Orders");
        //    });

        //    modelBuilder.Entity<Orders>(entity =>
        //    {
        //        entity.Property(e => e.Status).HasColumnType("nchar(50)");

        //        entity.HasOne(d => d.Store)
        //            .WithMany(p => p.Orders)
        //            .HasForeignKey(d => d.StoreId)
        //            .HasConstraintName("FK_Orders_Stores");
        //    });

        //    modelBuilder.Entity<Stores>(entity =>
        //    {
        //        entity.Property(e => e.Country).HasColumnType("varchar(50)");

        //        entity.Property(e => e.Name).HasColumnType("varchar(50)");
        //    });
        //}
    }
}
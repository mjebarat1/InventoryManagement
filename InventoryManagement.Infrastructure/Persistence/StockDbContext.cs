using InventoryManagement.Domain.Articles;
using InventoryManagement.Domain.StockMovement;
using InventoryManagement.Domain.StockBucket;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Infrastructure.Persistence
{
    public sealed class StockDbContext : DbContext
    {
        public StockDbContext(DbContextOptions<StockDbContext> options)
            : base(options)
        {
        }

        public DbSet<Article> Articles { get; set; }

        public DbSet<StockMovement> StockMovements { get; set; }

        public DbSet<StockBucket> StockBuckets { get; set; }

        public DbSet<StockMovementLine> StockMovementLines { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(StockDbContext).Assembly);
        }
    }
}

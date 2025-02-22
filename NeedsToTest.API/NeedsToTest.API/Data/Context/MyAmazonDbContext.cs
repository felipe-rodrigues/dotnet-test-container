using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;
using NeedsToTest.API.Data.Entities;

namespace NeedsToTest.API.Data.Context
{
    public class MyAmazonDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }

        public MyAmazonDbContext()
        {
            
        }

        public MyAmazonDbContext(DbContextOptions<MyAmazonDbContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Product>().ToCollection("Products");
        }
    }
}

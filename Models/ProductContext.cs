using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Webshop;

namespace Webshop;


    public class ProductContext : DbContext
    {
        public ProductContext(DbContextOptions<ProductContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Product>().HasMany(p => p.Category).WithMany(e => e.Product)
                .UsingEntity("product_productcategory");
        }

        public DbSet<Product> Product { get; set; }

        public DbSet<Category> Categories { get; set; }
    }
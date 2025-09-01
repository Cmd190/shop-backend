using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Webshop;

namespace WebApp1.Controllers;

public partial class ProductsController
{
    public class ProductContext : DbContext
    {
        public ProductContext(DbContextOptions<ProductContext> options) : base(options)
        {

        }

        public DbSet<Product> Product { get; set; }

        public DbSet<Category> Categories { get; set; }
    }
}
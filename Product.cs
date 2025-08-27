using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp1.Controllers;

public partial class ProductsController
{
    public class Product
    {
        public int Id { get; set; }

        [Column("ProductName")]
        public string Name { get; set; } = "";

        [Column("ProductDescription")]
        public string Description { get; set; } = "";
        public double Price { get; set; }
        public string Category { get; set; } = "";

        public static Product Default()
        {
            return new Product
            {
                Id = -1,
                Name = "Default",
                Description = "",
                Price = 0.0,
                Category = "None"
            };
        }
    }
}
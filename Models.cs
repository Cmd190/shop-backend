using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace Webshop;

[Table("productcategory")]
public class Category
{
    public int  Id { get; set; }

    public string Name { get; set; }
}

public class ProductOption
{
    public int  Id { get; set; }

    [Column("OptionValue")]
    public string Option { get; set; }
}

    public class Product
    {
        public int Id { get; set; }

        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("ProductName")]
        public string Name { get; set; } = "";

        [Column("ProductDescription")]
        public string Description { get; set; } = "";
        public double Price { get; set; }

        [Column("discount_percent")]

        public double DiscountPercent { get; set; }
        public int Category { get; set; }

        public string Manufacturer { get; set; }

        [Column("ship_duration")]
        public int ShipDuration { get; set; }

        [Column("stock_count")]
        public int StockCount { get; set; }

        [Column("product_option")]
        public int? Option { get; set; }

        public static Product Default()
        {
            return new Product
            {
                Id = -1,
                Name = "Default",
                Description = "",
                Price = 0.0,
                Category = -1
            };
        }
    }

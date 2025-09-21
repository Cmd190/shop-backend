using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Webshop;




public class ProductDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double Price { get; set; }
    public double DiscountPercent { get; set; }
    public string Manufacturer { get; set; } = string.Empty;
    public int ShipDuration { get; set; }
    public int StockCount { get; set; }
    public string ProductLink { get; set; } = string.Empty;
    public IEnumerable<string> Categories { get; set; } = [];
}

public class Product
{
    public int Id { get; set; }

    [Column("product_id")] public int ProductId { get; set; }

    [Column("ProductName")]
    [MaxLength(200)]
    public string Name { get; set; } = "";

    [Column("ProductDescription")] public string Description { get; set; } = "";
    public double Price { get; set; }

    [Column("discount_percent")] public double DiscountPercent { get; set; }

    [MaxLength(200)]
    public required string Manufacturer { get; set; }

    [Column("ship_duration")] public int ShipDuration { get; set; }

    [Column("stock_count")] public int StockCount { get; set; }

    [Column("product_option")] public int? Option { get; set; }

    [Column("product_link")]
    [MaxLength(200)]
    public required string ProductLink { get; set; }

    public List<Category> Category { get; set; } = [];

    public ProductDto ToProductDto()
    {
        return new ProductDto
        {
            Id = Id,
            Name = Name,
            ProductId = ProductId,
            Description = Description,
            Categories = Category.Select(c => c.Name),
            Price = Price,
            DiscountPercent = DiscountPercent,
            ShipDuration = ShipDuration,
            ProductLink = ProductLink,
            Manufacturer = Manufacturer,
            StockCount = StockCount
        };
    }

    public static Product Default()
    {
        return new Product
        {
            Id = -1,
            Name = "Default",
            Description = string.Empty,
            Price = 0.0,
            Manufacturer = string.Empty,
            ProductLink = string.Empty,
        };
    }
}
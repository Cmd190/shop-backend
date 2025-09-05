using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Webshop;

[ApiController]
[Route("[controller]")]
public class ProductsController(ILogger<ProductsController> logger, ProductContext context) : ControllerBase
{
    // TODD https://www.entityframeworktutorial.net/efcore/querying-in-ef-core.aspx

    private readonly ILogger<ProductsController> _logger = logger;

    [HttpGet("{categoryName}")]
    public async Task<ActionResult<IEnumerable<Product>>>  GetAllProducts(string categoryName)
    {
        return await context.Product
            .Where(p => p.Category.Any(c => c.Name.Equals(categoryName.Trim().ToLowerInvariant())))
            .ToListAsync();

    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await context.Product.FindAsync(id);
        return product == null ? NotFound() : product;
    }

}
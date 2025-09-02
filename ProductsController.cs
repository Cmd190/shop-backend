using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Webshop;

[ApiController]
[Route("[controller]")]
public partial class ProductsController : ControllerBase
{
    // TODD https://www.entityframeworktutorial.net/efcore/querying-in-ef-core.aspx

    private readonly ILogger<ProductsController> _logger;
    private readonly ProductContext _context;

    public ProductsController(ILogger<ProductsController> logger, ProductContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet("{categoryName}")]
    public async Task<ActionResult<IEnumerable<Product>>>  GetAllProducts(string categoryName)
    {
        return await _context.Product
            .Where(p => p.Category.Any(c => c.Name.Equals(categoryName.Trim().ToLowerInvariant())))
            .ToListAsync();

        // return await _context.Product.Take(3).ToListAsync();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await _context.Product.FindAsync(id);
        return product == null ? NotFound() : product;
    }
}
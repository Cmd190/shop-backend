using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Webshop;

namespace WebApp1.Controllers;

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
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Name == categoryName);
        return category == null ? NotFound()
            :await _context.Product
            .Where(p => p.Id == category.Id)
            .ToListAsync();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await _context.Product.FindAsync(id);
        return product == null ? NotFound() : product;
    }
}
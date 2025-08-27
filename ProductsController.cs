using Microsoft.AspNetCore.Mvc;

namespace WebApp1.Controllers;

[ApiController]
[Route("[controller]")]
public partial class ProductsController : ControllerBase
{

    private readonly ILogger<ProductsController> _logger;
    private readonly ProductContext _context;

    public ProductsController(ILogger<ProductsController> logger, ProductContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet]
    public IEnumerable<Product> GetByCategory(string category)
    {
        return new List<Product>();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await _context.Product.FindAsync(id);
        return product == null ? NotFound() : product;
    }

    [HttpGet]
    public Product GetByName(string name)
    {
        return Product.Default();
    }

}
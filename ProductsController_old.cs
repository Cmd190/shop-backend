using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Webshop;


[ApiController]
[Route("v1/[controller]")]
public class V1ProductsController(ILogger<V1ProductsController> logger, ProductContext context) : ControllerBase
{


    private readonly ILogger<V1ProductsController> _logger = logger;

    [HttpGet("{categoryName}")]
    public async Task<ActionResult<IEnumerable<ProductDto>>>  GetAllProducts(string categoryName)
    {
        return await context.Product
            .Include(p => p.Category)
            .Where(p => p.Category.Any(c
                => EF.Functions.Like(c.Name.ToLower(), categoryName.Trim().ToLowerInvariant())))
            .Select(p => p.ToProductDto())
            .ToListAsync();

    }


    [HttpGet]
    public async Task<ActionResult<ProductDto>> GetProduct(int? id, string? name)
    {
        if (id is > 0)
        {
            var product = await context.Product
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            return product == null ? NotFound() : product.ToProductDto();
        }

        if(!string.IsNullOrWhiteSpace(name))
        {
            var product = await context.Product
                .Include(p => p.Category)
                .Where(p => p.ProductLink.Equals(name.Trim().ToLowerInvariant())).FirstOrDefaultAsync();

            return product == null ? NotFound() : product.ToProductDto();

        }

        return BadRequest();

    }

}
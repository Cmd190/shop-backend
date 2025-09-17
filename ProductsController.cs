using Microsoft.AspNetCore.Mvc;

namespace Webshop.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductsController(ILogger<ProductsController> logger, ProductContext context, IRepositoryWrapper repo) : ControllerBase
{
    // TODD https://www.entityframeworktutorial.net/efcore/querying-in-ef-core.aspx
    // https://code-maze.com/searching-aspnet-core-webapi/


    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProducts()
    {

            var products = await repo.Product.GetAllProductsAsync();
            return Ok(products.Select(p => p.ToProductDto()));
    }



    [HttpGet("{categoryName}")]
    public async Task<ActionResult<IEnumerable<ProductDto>>>  GetAllProducts(string categoryName)
    {
        var products = await repo.Product.GetProductsByCategoryAsync(categoryName);
        return Ok(products.Select(p => p.ToProductDto()));
    }

    [HttpGet]
    public async Task<ActionResult<ProductDto>> GetProduct(int? id, string? name)
    {
        if (id is > 0)
        {
            var product = await repo.Product.GetProductByIdAsync((int) id);
            return product == null ? NotFound() : product.ToProductDto();
        }

        if(!string.IsNullOrWhiteSpace(name))
        {
            var product = await repo.Product.GetProductByNameAsync(name);
            return product == null ? NotFound() : product.ToProductDto();
        }

        return BadRequest();

    }
    
}
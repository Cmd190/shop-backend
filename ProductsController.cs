using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Webshop.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductsController(ILogger<ProductsController> logger, ProductContext context, IRepositoryWrapper repo) : ControllerBase
{
    // TODO Result Validation
    // TODD https://www.entityframeworktutorial.net/efcore/querying-in-ef-core.aspx
    // https://code-maze.com/searching-aspnet-core-webapi/


    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProducts([FromQuery] ProductQueryParams queryParams)
    {
        if (!ValidateFilterParams(queryParams))
        {
            return BadRequest("Incorrect Filter Settings");
        }

        var products = await repo.Product.GetAllProductsAsync(queryParams);
        var metadata = new
        {
            products.ResultCount,
            products.PageSize,
            products.CurrentPage,
            products.TotalPages,
            products.HasNextPage,
            products.HasPreviousPage
        };

        Response.Headers.Append("Pagination", JsonConvert.SerializeObject(metadata));

        logger.LogInformation("Returned {ProductsTotalCount} products from database.", products.ResultCount);
        return Ok(products.Select(p => p.ToProductDto()));
    }

    private static bool ValidateFilterParams(ProductQueryParams queryParams)
    {
        return queryParams.MinPrice > 0
               && queryParams.MinPrice < queryParams.MaxPrice
               && ValidateManufacturer(queryParams.Manufacturer)
               && ValidateCategory(queryParams.Category);
    }

    private static bool ValidateCategory(string? category) => category == null || category.Length is > 4 and < 100;

    private static bool ValidateManufacturer(string? manufacturer) => manufacturer == null || manufacturer.Length is > 4 and < 100;


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


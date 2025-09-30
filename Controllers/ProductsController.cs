using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Webshop;

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
        LogRequest();
        logger.LogInformation($"Received Request mapped to method {nameof(GetAllProducts)} with parameters {queryParams}");
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
        return queryParams.MinPrice >= 0
                     && queryParams.MinPrice < queryParams.MaxPrice
                     && ValidateName(queryParams.ProductName);
    }

    private static bool ValidateName(string? name) => name == null || name.Length is > 0 and < 200;



    [HttpGet("{categoryName}")]
    public async Task<ActionResult<IEnumerable<ProductDto>>>  GetAllProducts(string categoryName)
    {
        LogRequest();
        logger.LogInformation($"Received Request: Get Products by category method {nameof(GetAllProducts)} with parameters {nameof(categoryName)} {categoryName}");

        var products = await repo.Product.GetProductsByCategoryAsync(categoryName);
        return Ok(products.Select(p => p.ToProductDto()));
    }

    private void LogRequest()
    {
        var request = HttpContext.Request;
        var url = $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}";
        logger.LogInformation($"Request URL: {url}");
    }

    [HttpGet]
    public async Task<ActionResult<ProductDto>> GetProduct(int? id, string? link, string? name)
    {
        LogRequest();
        logger.LogInformation($"Received Request: Get single Product method {nameof(GetProduct)} with parameters {nameof(id)}:{id}, {nameof(link)}:{link}, {nameof(name)}:{name}");

        if (id is > 0)
        {
            var product = await repo.Product.GetProductByIdAsync((int) id);
            return product == null ? NotFound() : product.ToProductDto();
        }

        if(!string.IsNullOrWhiteSpace(link))
        {
            var product = await repo.Product.GetProductByLinkAsync(link);
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


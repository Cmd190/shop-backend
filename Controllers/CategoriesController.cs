using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Webshop.Models;

namespace Webshop.Controllers;
[ApiController]
[Route("[controller]")]
public class CategoriesController(ILogger<CategoriesController> logger, ProductContext context) : ControllerBase
{
    private readonly ILogger<CategoriesController> _logger = logger;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<string>>> GetAllCategories()
    {
        var categories = await context.Categories.Select(c => c.Name).ToListAsync();
        return Ok(categories);
    }
}
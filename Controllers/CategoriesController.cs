using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Webshop.Models;

namespace Webshop.Controllers;
[ApiController]
[Route("[controller]")]
public class CategoriesController(ILogger<CategoriesController> logger, ProductContext context) : ControllerBase
{

    //TODO introduce category dto

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Category>>> GetAllCategories()
    {
        var categories = await context.Categories.ToListAsync();
        return Ok(categories);
    }

    [HttpPost]
    public async Task<ActionResult<Category>> PostCategory(Category cat)
    {
          await context.Categories.AddAsync(cat);
          await context.SaveChangesAsync();
          return CreatedAtAction(cat.Name, new { id = cat.Id }, cat);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutCategory(int id, Category category)
    {
        if (id != category.Id)
        {
            return BadRequest();
        }

        context.Entry(category).State = EntityState.Modified;

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException e)
        {
            if (! await context.Categories.AnyAsync(c => c.Id == id))
            {
                return NotFound();
            }

            logger.LogError($"Error while calling {nameof(PutCategory)} with {category}");
        }

        return NoContent();
    }
}
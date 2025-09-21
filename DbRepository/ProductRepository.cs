using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Webshop.Controllers;

namespace Webshop;

public interface IProductRepository : IRepositoryBase<Product>
{
    Task<PagedList<Product>> GetAllProductsAsync(ProductQueryParams queryParams);

    Task<IEnumerable<Product>> GetProductsByCategoryAsync(string categoryName);

   Task<Product?> GetProductByIdAsync(int id);
   Task<Product?> GetProductByNameAsync(string name);

}

internal class ProductRepository(ProductContext context) : RepositoryBase<Product>(context), IProductRepository
{
    private readonly ProductContext _context = context;

    public async Task<Product?> GetProductByIdAsync(int id) =>
        await FindByCondition(p => p.Id == id )
        .Include(p => p.Category)
        .FirstOrDefaultAsync() ;

    public async Task<Product?> GetProductByNameAsync(string name) =>
        await FindByCondition(p
                => EF.Functions.Like(p.Name.ToLower(), name.Trim().ToLowerInvariant()))
            .Include(p => p.Category)
            .FirstOrDefaultAsync();

    public async Task<PagedList<Product>> GetAllProductsAsync(ProductQueryParams queryParams)
    {
        return await _context.Product
            .Include(p => p.Category)
            .Where(p =>
                p.Price > queryParams.MinPrice
                && p.Price < queryParams.MaxPrice
                // check manufacturer
                && (string.IsNullOrWhiteSpace(queryParams.Manufacturer)
                    ||  EF.Functions.Like(
                        p.Manufacturer.ToLower(),
                        queryParams.Manufacturer.Trim().ToLowerInvariant()
                    )
                )
                // check category
                && (string.IsNullOrWhiteSpace(queryParams.Category)
                    || p.Category.Any(c
                        => EF.Functions.Like(c.Name.ToLower(),
                            queryParams.Category.Trim().ToLowerInvariant()))
                )
                // check name
                // TODO implements startsWith
                && (string.IsNullOrWhiteSpace(queryParams.ProductName)
                    || EF.Functions.Like(p.Name.ToLower(),
                            queryParams.ProductName.Trim().ToLowerInvariant())
                )
            )

            .OrderBy(p => p.Name)
            .ToPagedListAsync(queryParams.PageNumber, queryParams.PageSize);
    }

    public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(string categoryName)
    {
        return await _context.Product
            .Include(p => p.Category)
            .Where(p => p.Category.Any(c
                => EF.Functions.Like(c.Name.ToLower(), categoryName.Trim().ToLowerInvariant())))
            .ToListAsync();
    }
}
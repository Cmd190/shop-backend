using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Webshop;

public interface IProductRepository : IRepositoryBase<Product>
{
    Task<PagedList<Product>> GetAllProductsAsync(ProductQueryParams queryParams);

    Task<IEnumerable<Product>> GetProductsByCategoryAsync(string categoryName);

   Task<Product?> GetProductByIdAsync(int id);
   Task<Product?> GetProductByNameAsync(string name);
   Task<Product?> GetProductByLinkAsync(string link);

}

internal class ProductRepository(ProductContext context) : RepositoryBase<Product>(context), IProductRepository
{
    private readonly ProductContext _context = context;

    public async Task<Product?> GetProductByIdAsync(int id) =>
        await FindByCondition(p => p.Id == id )
        .Include(p => p.Category)
        .AsNoTracking()
        .FirstOrDefaultAsync() ;

    public async Task<Product?> GetProductByNameAsync(string name) =>
        await FindByCondition(p
                => EF.Functions.Like(p.Name.ToLower(), name.Trim().ToLowerInvariant()))
            .Include(p => p.Category)
            .AsNoTracking()
            .FirstOrDefaultAsync();
    public async Task<Product?> GetProductByLinkAsync(string link) =>
        await FindByCondition(p
                => EF.Functions.Like(p.ProductLink.ToLower(), link.Trim().ToLowerInvariant()))
            .Include(p => p.Category)
            .AsNoTracking()
            .FirstOrDefaultAsync();
    public async Task<PagedList<Product>> GetAllProductsAsync(ProductQueryParams queryParams)
    {
        return await _context.Product
            .Include(p => p.Category)
            .Where(p =>
                p.Price > queryParams.MinPrice
                && p.Price < queryParams.MaxPrice
                // check manufacturer
                && ( queryParams.Manufacturers == null || !queryParams.Manufacturers.Any()
                    ||  queryParams.Manufacturers.Any(m =>  EF.Functions.Like(m.ToLower(),
                        p.Manufacturer.ToLower()))
                    )

                // check category
                && (queryParams.Categories == null || !queryParams.Categories.Any()
                    || p.Category.Any(c
                        => queryParams.Categories.Contains(c.Name.Trim().ToLower()))
                )
                // check name
                && (string.IsNullOrWhiteSpace(queryParams.ProductName)
                    || EF.Functions.Like(p.Name.ToLower(),
                            queryParams.ProductName.Trim().ToLowerInvariant() + "%")
                )
            )

            .OrderBy(p => p.Name)
            .AsNoTracking()
            .ToPagedListAsync(queryParams.PageNumber, queryParams.PageSize);
    }

    public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(string categoryName)
    {
        return await _context.Product
            .Include(p => p.Category)
            .Where(p => p.Category.Any(c
                => EF.Functions.Like(c.Name.ToLower(), categoryName.Trim().ToLowerInvariant())))
            .AsNoTracking()
            .ToListAsync();
    }
}
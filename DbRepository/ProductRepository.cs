using Microsoft.EntityFrameworkCore;

namespace Webshop;

public interface IProductRepository : IRepositoryBase<Product>
{
    Task<IEnumerable<Product>> GetAllProductsAsync();

    Task<IEnumerable<Product>> GetProductsByCategoryAsync(string categoryName);

   Task<Product?> GetProductByIdAsync(int id);
   Task<Product?> GetProductByNameAsync(string name);

}

internal class ProductRepository(ProductContext context) : RepositoryBase<Product>(context), IProductRepository
{
    private readonly ProductContext _context = context;
    public async Task<IEnumerable<Product>> GetAllProductsAsync() => await FindAll().ToListAsync();

    public async Task<Product?> GetProductByIdAsync(int id) =>
        await FindByCondition(p => p.Id == id )
        .Include(p => p.Category)
        .FirstOrDefaultAsync() ;

    public async Task<Product?> GetProductByNameAsync(string name) =>
        await FindByCondition(p
                => EF.Functions.Like(p.Name.ToLower(), name.Trim().ToLowerInvariant()))
            .Include(p => p.Category)
            .FirstOrDefaultAsync();

    public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(string categoryName) =>
        await _context.Product
            .Include(p => p.Category)
            .Where(p => p.Category.Any(c
                => EF.Functions.Like(c.Name.ToLower(), categoryName.Trim().ToLowerInvariant())))
            .ToListAsync();

}
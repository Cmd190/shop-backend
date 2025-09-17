using Microsoft.EntityFrameworkCore;

namespace Webshop;

internal interface IProductRepository : IRepositoryBase<Product>
{
    Task<IEnumerable<Product>> GetAllProductsAsync();
    Task<Product?> GetProductByIdAsync(int Id);

    Task<IEnumerable<Product>> GetProductsByCategory(string categoryName);

}

internal class ProductRepository(ProductContext context) : RepositoryBase<Product>(context), IProductRepository
{
    public async Task<IEnumerable<Product>> GetAllProductsAsync() => await FindAll().ToListAsync();

    public async Task<Product?> GetProductByIdAsync(int id) => await FindByCondition(p => p.Id == id )
        .Include(p => p.Category)
        .FirstOrDefaultAsync() ;

    public async Task<IEnumerable<Product>> GetProductsByCategory(string categoryName)
    {
        return await context.Product
            .Include(p => p.Category)
            .Where(p => p.Category.Any(c
                => EF.Functions.Like(c.Name.ToLower(), categoryName.Trim().ToLowerInvariant())))
            .Select(p => p.ToProductDto())
            .ToListAsync();
    }
}
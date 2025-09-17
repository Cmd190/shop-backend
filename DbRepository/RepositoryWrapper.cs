namespace Webshop;

internal class RepositoryWrapper(ProductContext context) : IRepositoryWrapper
{
    public IProductRepository Product { get; } = new ProductRepository(context);

    public ICategoryRepository Category { get; } = new CategoryRepository(context);

    public async Task SaveAsync() => await context.SaveChangesAsync();
}
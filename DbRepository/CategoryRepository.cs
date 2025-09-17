namespace Webshop;

internal interface ICategoryRepository : IRepositoryBase<Category>;


internal class CategoryRepository(ProductContext context) : RepositoryBase<Category>(context), ICategoryRepository;
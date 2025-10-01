using Webshop.Models;

namespace Webshop.DbRepository;

public interface ICategoryRepository : IRepositoryBase<Category>;


internal class CategoryRepository(ProductContext context) : RepositoryBase<Category>(context), ICategoryRepository;
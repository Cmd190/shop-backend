using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Webshop;

public interface IRepositoryBase<T>
{
    IQueryable<T> FindAll();
    IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression);
    void Create(T entity);
    void Update(T entity);
    void Delete(T entity);
}

public interface IRepositoryWrapper
{
    IProductRepository Product { get; }
    ICategoryRepository Category { get; }
    Task SaveAsync();
}

public abstract class RepositoryBase<T>(ProductContext context) : IRepositoryBase<T> where T: class
{
    private DbSet<T> DbSet => context.Set<T>();

    public IQueryable<T> FindAll() => DbSet.AsNoTracking();

    public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression) => DbSet.Where(expression).AsNoTracking();

    public void Create(T entity) => DbSet.AddAsync(entity);

    public void Update(T entity) => DbSet.Update(entity);

    public void Delete(T entity) => DbSet.Remove(entity);
}
using CustomerTableTest.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CustomerTableTest.DAL.Repositories;

public class BaseRepository<T> : IBaseRepository<T> where T : class
{
    private readonly ApplicationDbContext _ctx;
    private readonly DbSet<T> _set;

    public BaseRepository(ApplicationDbContext ctx)
    {
        _ctx = ctx;
        _set = _ctx.Set<T>();
    }

    public Task<T?> GetByIdAsync(int id) => _set.FindAsync(id).AsTask();

    public IQueryable<T> Query(Expression<Func<T, bool>>? predicate = null)
        => predicate is null ? _set.AsQueryable() : _set.Where(predicate);

    public Task AddAsync(T entity) => _set.AddAsync(entity).AsTask();

    public Task UpdateAsync(T entity)
    {
        _set.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(T entity)
    {
        _set.Remove(entity);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync() => _ctx.SaveChangesAsync();
}

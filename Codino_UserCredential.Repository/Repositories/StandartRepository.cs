using System.Linq.Expressions;
using Codino_UserCredential.Repository.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Codino_UserCredential.Repository.Repositories;

public class StandartRepository<TEntity, TDbContext, TKey> : IStandartRepository<TEntity, TDbContext, TKey> where TEntity : class, IStandartModel<TKey> where TDbContext : DbContext where TKey : struct
{
    internal DbSet<TEntity> dbSet;
    private readonly TDbContext context;
    private bool disposed;

    public StandartRepository(TDbContext context)
    {
        this.context = context;
        dbSet = this.context.Set<TEntity>();
    }

    public virtual RepositoryQuery<TEntity, TDbContext, TKey> Query()
    {
        return new RepositoryQuery<TEntity, TDbContext, TKey>(this);
    }

    internal IEnumerable<TEntity> InternalGet(
        Expression<Func<TEntity, bool>> filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        List<Expression<Func<TEntity, object>>> includeProperties = null,
        int? page = null, int? pageSize = null 
    )
    {
        IQueryable<TEntity> query = dbSet;
        includeProperties?.ForEach(delegate(Expression<Func<TEntity, object>> i)
        {
            EntityFrameworkQueryableExtensions.Include(query, i);
        });
        if (filter != null)
        {
            query = query.Where(filter);
        }

        if (orderBy != null)
        {
            query = orderBy(query);
        }

        if (page.HasValue && pageSize.HasValue)
        {
            query = query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);
        }

        return query.ToList();
    }

    public IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "*")
    {
        return GetQuery(filter, orderBy, includeProperties).ToList();
    }

    public virtual async Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "*")
    {
        IQueryable<TEntity> queryable = dbSet;
        if (filter != null)
        {
            queryable = queryable.Where(filter);
        }

        string[] array = includeProperties.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var navigationPropertyPath in array)
        {
            queryable = EntityFrameworkQueryableExtensions.Include(queryable, navigationPropertyPath);
        }

        if (orderBy != null)
        {
            return await EntityFrameworkQueryableExtensions.ToListAsync(orderBy(queryable));
        }

        return await EntityFrameworkQueryableExtensions.ToListAsync(queryable);
    }

    public IQueryable<TEntity> GetQuery(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "*")
    {
        IQueryable<TEntity> queryable = dbSet;
        if (filter != null)
        {
            queryable = queryable.Where(filter);
        }

        string[] array = includeProperties.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string navigationPropertyPath in array)
        {
            queryable = EntityFrameworkQueryableExtensions.Include(queryable, navigationPropertyPath);
        }

        if (orderBy != null)
        {
            return orderBy(queryable);
        }

        return queryable;
    }
    

    public virtual TEntity GetById(TKey id)
    {
        return dbSet.Find(id);
    }

    public virtual async Task<TEntity> GetByIdAsync(TKey id)
    {
        return await dbSet.FindAsync(id);
    }

    public virtual void Insert(TEntity entity)
    {
        dbSet.Add(entity);
    }

    public virtual async void InsertAsync(TEntity entity)
    {
        await dbSet.AddAsync(entity);
    }

    public virtual void Delete(TKey id)
    {
        TEntity entityToDelete = dbSet.Find(id);
        Delete(entityToDelete);
    }

    public virtual void Delete(TEntity entityToDelete)
    {
        if (context.Entry(entityToDelete).State == EntityState.Detached)
        {
            dbSet.Attach(entityToDelete);
        }

        dbSet.Remove(entityToDelete);
    }

    public virtual async void DeleteAsync(TEntity id)
    {
        Delete(await dbSet.FindAsync(id));
    }

    public virtual void Update(TEntity entityToUpdate)
    {
        dbSet.Attach(entityToUpdate);
        context.Entry(entityToUpdate).State = EntityState.Modified;
    }

    public virtual void Update(TEntity entityToUpdate, List<string> fields)
    {
        EntityEntry<TEntity> entityEntry = dbSet.Attach(entityToUpdate);
        foreach (var field in fields)
        {
            if (entityToUpdate.GetType().GetProperty(field) != null)
            {
                entityEntry.Property(field).IsModified = true;
                continue;
            }

            throw new Exception(field + "is not a property of" + entityToUpdate.GetType().Name);
        }
    }

    public virtual void Update(TEntity entityToUpdate, params string[] fields)
    {
        EntityEntry<TEntity> entityEntry = dbSet.Attach(entityToUpdate);
        foreach (string text in fields)
        {
            if (entityToUpdate.GetType().GetProperty(text) != null)
            {
                entityEntry.Property(text).IsModified = true;
                continue;
            }

            throw new Exception(text + "is not a property of" + entityToUpdate.GetType().Name);
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing)
        {
            if (disposing)
            {
                context.Dispose();
            }

            disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing:true);
        GC.SuppressFinalize(this);
    }

    public bool Any(Expression<Func<TEntity, bool>> expression)
    {
        return dbSet.Any(expression);
    }
}
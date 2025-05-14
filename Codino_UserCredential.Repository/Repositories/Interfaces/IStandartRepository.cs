using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Codino_UserCredential.Repository.Repositories.Interfaces;

namespace Codino_UserCredential.Repository.Repositories
{
    public interface IStandartRepository<TEntity, TDbContext, TKey>
        where TEntity : class, IStandartModel<TKey>
        where TDbContext : DbContext
        where TKey : struct
    {
        // Sorgu oluşturucu
        RepositoryQuery<TEntity, TDbContext, TKey> Query();

        // Temel veri alma fonksiyonları
        IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = ""
        );

        Task<IEnumerable<TEntity>> GetAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = ""
        );

        IQueryable<TEntity> GetQuery(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = ""
        );

        TEntity GetById(TKey id);
        Task<TEntity> GetByIdAsync(TKey id);

        void Insert(TEntity entity);
        void InsertAsync(TEntity entity);

        void Delete(TKey id);
        void Delete(TEntity entityToDelete);
        void DeleteAsync(TEntity id);

        void Update(TEntity entityToUpdate);
        void Update(TEntity entityToUpdate, List<string> fields);
        void Update(TEntity entityToUpdate, params string[] fields);

        bool Any(Expression<Func<TEntity, bool>> expression);
    }
}
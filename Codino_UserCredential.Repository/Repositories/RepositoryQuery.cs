using System.Linq.Expressions;
using Codino_UserCredential.Repository.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Codino_UserCredential.Repository.Repositories
{
    public sealed class RepositoryQuery<TEntity, TDbContext, TKey>
        where TEntity : class, IStandartModel<TKey>
        where TDbContext : DbContext
        where TKey : struct
    {
        private readonly List<Expression<Func<TEntity, object>>> _includeProperties;
        private readonly StandartRepository<TEntity, TDbContext, TKey> _repository;
        private Expression<Func<TEntity, bool>> _filter;
        private Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> _orderBy;
        private int? _page;
        private int? _pageSize;

        public RepositoryQuery(StandartRepository<TEntity, TDbContext, TKey> repository)
        {
            _repository = repository;
            _includeProperties = new List<Expression<Func<TEntity, object>>>();
        }

        public RepositoryQuery<TEntity, TDbContext, TKey> Filter(Expression<Func<TEntity, bool>> filter)
        {
            _filter = filter;
            return this;
        }

        public RepositoryQuery<TEntity, TDbContext, TKey> OrderBy(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy)
        {
            _orderBy = orderBy;
            return this;
        }

        public RepositoryQuery<TEntity, TDbContext, TKey> Include(Expression<Func<TEntity, object>> expression)
        {
            _includeProperties.Add(expression);
            return this;
        }

        public IEnumerable<TEntity> GetPage(int page, int pageSize, out int totalCount)
        {
            _page = page;
            _pageSize = pageSize;
            totalCount = _repository.Get(_filter).Count();
            return _repository.InternalGet(_filter, _orderBy, _includeProperties, _page, _pageSize);
        }

        public IEnumerable<TEntity> Get()
        {
            return _repository.InternalGet(_filter, _orderBy, _includeProperties, _page, _pageSize);
        }
    }
}

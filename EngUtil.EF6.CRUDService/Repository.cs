// --------------------------------------------------------------------------------
// <copyright filename="Repository.cs" date="20-06-2016">(c) 2016 All Rights Reserved</copyright>
// <author>Oliver Engels</author>
// --------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using EngUtil.EF6.CRUDService.Helper;


namespace EngUtil.EF6.CRUDService
{
    public abstract class Repository<TDbContext, TEntity, TModel> : IRepository<TModel>, IRepositoryDto<TEntity, TModel>
        where TDbContext : DbContext
    {
        #region ctor

        public Repository(string connectionString)
        {
            var context = (TDbContext) Activator.CreateInstance(typeof(TDbContext), connectionString);
            DbContextService = new DbContextSessionInternal<TDbContext>(context);
        }

        public Repository(TDbContext dbContext)
        {
            DbContextService = new DbContextSessionInternal<TDbContext>(dbContext);
        }

        public Repository(IDbContextService contextService)
        {
            DbContextService = contextService;
        }


        #endregion

        #region properties : protected

        internal IDbContextService DbContextService { get; set; }   

        #endregion

        #region properties

        public virtual Expression<Func<TModel, TEntity>> AsEntityExpression { get; set; }

        public virtual Expression<Func<TEntity, TModel>> AsModelExpression { get; set; }

        #endregion

        #region mthods

        public virtual TEntity AsEntity(TModel model)
        {
            return AsEntityExpression.Compile().Invoke(model);
        }

        public virtual TModel AsModel(TEntity entity)
        {
            return AsModelExpression.Compile().Invoke(entity);
        }

        /// <inheritdoc>
        public virtual int Count(Expression<Func<TModel, bool>> filter = null)
        {
            using (var ctx = DbContextService.CreateContext())
            {
                IQueryable<TModel> query;
                var queryableSet = ctx.GetDbSetAsIQuariable<TEntity>();
                if (queryableSet == null)
                    throw new NullReferenceException($"Could not found DbSet of Entity-Type { typeof(TEntity).Name } in DbContext!");
                if (filter != null)
                    query = queryableSet.Select(AsModelExpression).Where(filter);
                else
                    query = queryableSet.Select(AsModelExpression);
                return query.Count();
            }
        }

        /// <inheritdoc>
        public virtual async Task<int> CountAsync(Expression<Func<TModel, bool>> filter = null)
        {
            using (var ctx = DbContextService.CreateContext())
            {
                IQueryable<TModel> query;
                var queryableSet = ctx.GetDbSetAsIQuariable<TEntity>();
                if (queryableSet == null)
                    throw new NullReferenceException($"Could not found DbSet of Entity-Type { typeof(TEntity).Name } in DbContext!");
                if (filter != null)
                    query = queryableSet.Select(AsModelExpression).Where(filter);
                else
                    query = queryableSet.Select(AsModelExpression);      
                return await query.CountAsync();
            }
        }

        /// <inheritdoc>
        public virtual IEnumerable<TModel> Get(Expression<Func<TModel, bool>> filter = null, Func<IQueryable<TModel>, IOrderedQueryable<TModel>> orderBy = null, int skip = 0, int take = 0)
        {
            using (var ctx = DbContextService.CreateContext())
            {
                IQueryable<TModel> query;
                var queryableSet = ctx.GetDbSetAsIQuariable<TEntity>();
                if (queryableSet == null)
                    throw new NullReferenceException($"Could not found DbSet of Entity-Type { typeof(TEntity).Name } in DbContext!");
                if (filter != null)
                    query = queryableSet.Select(AsModelExpression).Where(filter);
                else
                    query = queryableSet.Select(AsModelExpression);
                if (orderBy != null)
                {
                    query = orderBy(query);
                    if (skip > 0)
                        query = query.Skip(skip);
                    if (take > 0)
                        query = query.Take(take);
                }
                return query.ToList();
            }
        }

        /// <inheritdoc>
        public virtual async Task<IEnumerable<TModel>> GetAsync(Expression<Func<TModel, bool>> filter = null, Func<IQueryable<TModel>, IOrderedQueryable<TModel>> orderBy = null, int skip = 0, int take = 0)
        {
            using (var ctx = DbContextService.CreateContext())
            {
                IQueryable<TModel> query;
                var queryableSet = ctx.GetDbSetAsIQuariable<TEntity>();
                if (queryableSet == null)
                    throw new NullReferenceException($"Could not found DbSet of Entity-Type { typeof(TEntity).Name } in DbContext!");
                if (filter != null)
                    query = queryableSet.Select(AsModelExpression).Where(filter);
                else
                    query = queryableSet.Select(AsModelExpression);
                if (orderBy != null)
                {
                    query = orderBy(query);
                    if (skip > 0)
                        query = query.Skip(skip);
                    if (take > 0)
                        query = query.Take(take);
                }
                return await query.ToListAsync();
            }
        }

        /// <inheritdoc>
        public virtual TModel GetFirst(Expression<Func<TModel, bool>> filter)
        {
            using (var ctx = DbContextService.CreateContext())
            {
                IQueryable<TModel> query;
                var queryableSet = ctx.GetDbSetAsIQuariable<TEntity>();
                if (queryableSet == null)
                    throw new NullReferenceException($"Could not found DbSet of Entity-Type { typeof(TEntity).Name } in DbContext!");
                if (filter != null)
                    query = queryableSet.Select(AsModelExpression).Where(filter);
                else
                    query = queryableSet.Select(AsModelExpression);
                return query.FirstOrDefault();
            }
        }

        /// <inheritdoc>
        public virtual async Task<TModel> GetFirstAsync(Expression<Func<TModel, bool>> filter)
        {
            using (var ctx = DbContextService.CreateContext())
            {
                IQueryable<TModel> query;
                var queryableSet = ctx.GetDbSetAsIQuariable<TEntity>();
                if (queryableSet == null)
                    throw new NullReferenceException($"Could not found DbSet of Entity-Type { typeof(TEntity).Name } in DbContext!");
                if (filter != null)
                    query = queryableSet.Select(AsModelExpression).Where(filter);
                else
                    query = queryableSet.Select(AsModelExpression);
                return await query.FirstOrDefaultAsync();
            }
        }

        /// <inheritdoc>
        public virtual TModel Insert(TModel model)
        {
            using (var ctx = DbContextService.CreateContext())
            {
                var dbSet = ctx.Set(typeof(TEntity));
                var entity = dbSet.Add(AsEntity(model));
                ctx.SaveChanges();
                model = AsModel((TEntity)entity);
                return model;
            };
        }

        /// <inheritdoc>
        public virtual async Task<TModel> InsertAsync(TModel model)
        {
            using (var ctx = DbContextService.CreateContext())
            {
                var dbSet = ctx.Set(typeof(TEntity));
                var entity = dbSet.Add(AsEntity(model));
                await ctx.SaveChangesAsync();
                model = AsModel((TEntity)entity);
                return model;
            };
        }

        /// <inheritdoc>
        public virtual void Update(TModel model)
        {
            using (var ctx = DbContextService.CreateContext())
            {
                var dbSet = ctx.Set(typeof(TEntity));
                var newEntityState = AsEntity(model);
                var entity = dbSet.Find(GetPrimaryKeyValues(newEntityState));
                ctx.Entry(entity).CurrentValues.SetValues(newEntityState);
                ctx.SaveChanges();
            }
        }

        /// <inheritdoc>
        public virtual async Task UpdateAsync(TModel model)
        {
            using (var ctx = DbContextService.CreateContext())
            {
                var dbSet = ctx.Set(typeof(TEntity));
                var newEntityState = AsEntity(model);
                var entity = await dbSet.FindAsync(GetPrimaryKeyValues(newEntityState));
                ctx.Entry(entity).CurrentValues.SetValues(newEntityState);
                await ctx.SaveChangesAsync();
            }
        }

        /// <inheritdoc>
        public virtual void Delete(TModel model)
        { 
            using (var ctx = DbContextService.CreateContext())
            {
                var entityToDelete = AsEntity(model);
                var dbSet = ctx.Set(typeof(TEntity));
                var entity = dbSet.Find(GetPrimaryKeyValues(entityToDelete));            
                dbSet.Attach(entity);
                dbSet.Remove(entity);
                ctx.SaveChanges();
            }
        }

        /// <inheritdoc>
        public virtual async Task DeleteAsync(TModel model)
        {
            using (var ctx = DbContextService.CreateContext())
            {
                var entityToDelete = AsEntity(model);
                var dbSet = ctx.Set(typeof(TEntity));
                var entity = dbSet.Find(GetPrimaryKeyValues(entityToDelete));
                dbSet.Attach(entity);
                dbSet.Remove(entity);
                await ctx.SaveChangesAsync();
            }
        }

        /// <inheritdoc>
        public virtual void Delete(object[] key)
        {
            using (var ctx = DbContextService.CreateContext())
            {     
                var dbSet = ctx.Set(typeof(TEntity));
                var entity = dbSet.Find(key);
                dbSet.Attach(entity);
                dbSet.Remove(entity);
                ctx.SaveChanges();
            }
        }

        /// <inheritdoc>
        public virtual async Task DeleteAsync(object[] key)
        {
            using (var ctx = DbContextService.CreateContext())
            {
                var dbSet = ctx.Set(typeof(TEntity));
                var entity = dbSet.Find(key);
                dbSet.Attach(entity);
                dbSet.Remove(entity);
                await ctx.SaveChangesAsync();
            }
        }

        /// <inheritdoc>
        public virtual void Delete(object key)
        {
            using (var ctx = DbContextService.CreateContext())
            {
                var dbSet = ctx.Set(typeof(TEntity));
                var entity = dbSet.Find(key);
                dbSet.Attach(entity);
                dbSet.Remove(entity);
                ctx.SaveChanges();
            }
        }

        /// <inheritdoc>
        public virtual async Task DeleteAsync(object key)
        {
            using (var ctx = DbContextService.CreateContext())
            {
                var dbSet = ctx.Set(typeof(TEntity));
                var entity = dbSet.Find(key);
                dbSet.Attach(entity);
                dbSet.Remove(entity);
                await ctx.SaveChangesAsync();
            }
        }

        /// <inheritdoc>
        public TDbContext GetContext()
        {
            return (TDbContext)DbContextService.CreateContext();            
        }

        #endregion

        #region mthods: private     

        private object[] GetPrimaryKeyValues(object entity)
        {
            return typeof(TEntity)
                .GetProperties()
                .Where(x => x.CustomAttributes.Count() > 0 && x.GetCustomAttributes<KeyAttribute>().Count() > 0)
                .Select(x => (
                    Property: x,
                    KeyOrder: x.GetCustomAttributes<ColumnAttribute>().Count() > 0
                                ? x.GetCustomAttributes<ColumnAttribute>().ToList()[0].Order
                                : -1
                ))
                .OrderBy(x => x.KeyOrder)
                .Select(x => x.Property.GetValue(entity))
                .ToArray();
        }

        #endregion
    }
}

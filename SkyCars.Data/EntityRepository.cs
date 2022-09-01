using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Transactions;
using LinqToDB;
using LinqToDB.Data;
using SkyCars.Core;
using System.Collections;
using SkyCars.Data.Extensions;
using SkyCars.Core.DomainEntity.Grid;
using SkyCars.Data;

namespace SkyCars.Data
{
    /// <summary>
    /// Represents the entity repository implementation
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public partial class EntityRepository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
    {
        #region Fields

        private readonly IDataProvider _dataProvider;

        #endregion

        #region Ctor

        public EntityRepository(IDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get all entity entries
        /// </summary>
        /// <param name="getAllAsync">Function to select entries</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the entity entries
        /// </returns>
        protected virtual async Task<IList<TEntity>> GetEntitiesAsync(Func<Task<IList<TEntity>>> getAllAsync)
        {
            return await getAllAsync();
        }

        /// <summary>
        /// Get all entity entries
        /// </summary>
        /// <param name="getAll">Function to select entries</param>
        /// <returns>Entity entries</returns>
        protected virtual IList<TEntity> GetEntities(Func<IList<TEntity>> getAll)
        {
            return getAll();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get the entity entry
        /// </summary>
        /// <param name="id">Entity entry identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the entity entry
        /// </returns>
        public virtual async Task<TEntity> GetByIdAsync(int? id)
        {
            if (!id.HasValue || id == 0)
                return null;

            async Task<TEntity> getEntityAsync()
            {
                return await Table.FirstOrDefaultAsync(entity => entity.Id == Convert.ToInt32(id));
            }

            return await getEntityAsync();
        }

        /// <summary>
        /// Get entity entries by identifiers
        /// </summary>
        /// <param name="ids">Entity entry identifiers</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the entity entries
        /// </returns>
        public virtual async Task<IList<TEntity>> GetByIdsAsync(IList<int> ids)
        {
            if (!ids?.Any() ?? true)
                return new List<TEntity>();

            async Task<IList<TEntity>> getByIdsAsync()
            {
                var query = Table;

                //get entries
                var entries = await query.Where(entry => ids.Contains(entry.Id)).ToListAsync();

                //sort by passed identifiers
                var sortedEntries = new List<TEntity>();
                foreach (var id in ids)
                {
                    var sortedEntry = entries.FirstOrDefault(entry => entry.Id == id);
                    if (sortedEntry != null)
                        sortedEntries.Add(sortedEntry);
                }

                return sortedEntries;
            }

            return await getByIdsAsync();
        }

        /// <summary>
        /// Get all entity entries
        /// </summary>
        /// <param name="func">Function to select entries</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the entity entries
        /// </returns>
        public virtual async Task<IList<TEntity>> GetAllAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>> func = null)
        {
            async Task<IList<TEntity>> getAllAsync()
            {
                var query = Table;
                query = func != null ? func(query) : query;

                return await query.ToListAsync();
            }

            return await GetEntitiesAsync(getAllAsync);
        }

        /// <summary>
        /// Get all entity entries
        /// </summary>
        /// <param name="func">Function to select entries</param>
        /// <returns>Entity entries</returns>
        public virtual IList<TEntity> GetAll(Func<IQueryable<TEntity>, IQueryable<TEntity>> func = null)
        {
            IList<TEntity> getAll()
            {
                var query = Table;
                query = func != null ? func(query) : query;

                return query.ToList();
            }

            return GetEntities(getAll);
        }

        /// <summary>
        /// Get all entity entries
        /// </summary>
        /// <param name="func">Function to select entries</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the entity entries
        /// </returns>
        public virtual async Task<IList<TEntity>> GetAllAsync(Func<IQueryable<TEntity>, Task<IQueryable<TEntity>>> func = null)
        {
            async Task<IList<TEntity>> getAllAsync()
            {
                var query = Table;
                query = func != null ? await func(query) : query;

                return await query.ToListAsync();
            }

            return await GetEntitiesAsync(getAllAsync);
        }

        /// <summary>
        /// Get paged list of all entity entries
        /// </summary>
        /// <param name="func">Function to select entries</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="getOnlyTotalCount">Whether to get only the total number of entries without actually loading data</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the paged list of entity entries
        /// </returns>
        public virtual async Task<IPagedList<TEntity>> GetAllPagedAsync(GridRequestModel objGrid, IQueryable<TEntity> query, Func<IQueryable<TEntity>, IQueryable<TEntity>> func = null)
        {
            if (query == null)
                query = Table;
            query = func != null ? func(query) : query;
            return await query.BuildPredicate(objGrid);
        }

        /// <summary>
        /// Get paged list of all entity entries
        /// </summary>
        /// <param name="func">Function to select entries</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="getOnlyTotalCount">Whether to get only the total number of entries without actually loading data</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the paged list of entity entries
        /// </returns>
        public virtual async Task<IPagedList<TEntity>> GetAllPagedAsync(Func<IQueryable<TEntity>, Task<IQueryable<TEntity>>> func = null,
            int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false)
        {
            var query = Table;

            query = func != null ? await func(query) : query;

            return await query.ToPagedListAsync(pageIndex, pageSize, getOnlyTotalCount);
        }

        /// <summary>
        /// Insert the entity entry
        /// </summary>
        /// <param name="entity">Entity entry</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task<TEntity> InsertAsync(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return await _dataProvider.InsertEntityAsync(entity);

            //event notification
            //Log entry
        }

        /// <summary>
        /// Insert entity entries
        /// </summary>
        /// <param name="entities">Entity entries</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertAsync(IList<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            await _dataProvider.BulkInsertEntitiesAsync(entities);
            transaction.Complete();

            //event notification
            //Log entry
        }

        /// <summary>
        /// Update the entity entry
        /// </summary>
        /// <param name="entity">Entity entry</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateAsync(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await _dataProvider.UpdateEntityAsync(entity);

            //event notification
            //Log entry
        }

        /// <summary>
        /// Update entity entries
        /// </summary>
        /// <param name="entities">Entity entries</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateAsync(IList<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            if (!entities.Any())
                return;

            await _dataProvider.BulkUpdateEntitiesAsync(entities);

            //event notification
            //Log entry
            /*foreach (var entity in entities)
                await _eventPublisher.EntityUpdatedAsync(entity);*/
        }

        /// <summary>
        /// Delete the entity entry
        /// </summary>
        /// <param name="entity">Entity entry</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteAsync(TEntity entity)
        {
            switch (entity)
            {
                case null:
                    throw new ArgumentNullException(nameof(entity));

                case ISoftDeletedEntity softDeletedEntity:
                    softDeletedEntity.Deleted = true;
                    await _dataProvider.UpdateEntityAsync(entity);
                    break;

                default:
                    await _dataProvider.DeleteEntityAsync(entity);
                    break;
            }

            //event notification
            //Log entry
        }

        /// <summary>
        /// Delete entity entries
        /// </summary>
        /// <param name="entities">Entity entries</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteAsync(IList<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            if (entities.OfType<ISoftDeletedEntity>().Any())
            {
                foreach (var entity in entities)
                    if (entity is ISoftDeletedEntity softDeletedEntity)
                    {
                        softDeletedEntity.Deleted = true;
                        await _dataProvider.UpdateEntityAsync(entity);
                    }
            }
            else
                await _dataProvider.BulkDeleteEntitiesAsync(entities);

            //event notification
            //Log entry
            /*foreach (var entity in entities)
                await _eventPublisher.EntityDeletedAsync(entity);*/
        }

        /// <summary>
        /// Delete entity entries by the passed predicate
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the number of deleted records
        /// </returns>
        public virtual async Task<int> DeleteAsync(Expression<Func<TEntity, bool>> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            return await _dataProvider.BulkDeleteEntitiesAsync(predicate);
        }

        /// <summary>
        /// Loads the original copy of the entity
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the copy of the passed entity
        /// </returns>
        public virtual async Task<TEntity> LoadOriginalCopyAsync(TEntity entity)
        {
            return await (await _dataProvider.GetTableAsync<TEntity>())
                .FirstOrDefaultAsync(e => e.Id == Convert.ToInt32(entity.Id));
        }

        /// <summary>
        /// Executes SQL command and returns number of affected records
        /// </summary>
        /// <param name="procedureName">Procedure name</param>
        /// <param name="parameters">Command parameters</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the number of records, affected by command execution.
        /// </returns>
        public virtual async Task<int> ExecuteSqlAsync(string procedureName, params DataParameter[] parameters)
        {
            return await _dataProvider.ExecuteNonQueryAsync(procedureName, parameters?.ToArray());
        }

        /// <summary>
        /// Executes SQL command and returns results as collection of values of specified type
        /// </summary>
        /// <param name="procedureName">Procedure name</param>
        /// <param name="parameters">Command parameters</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the entity entry
        /// </returns>
        public virtual async Task<TEntity> EntityFromSqlAsync(string procedureName, params DataParameter[] parameters)
        {
            return await _dataProvider.ExecuteNonQueryAsync<TEntity>(procedureName, parameters?.ToArray());
        }

        /// <summary>
        /// Executes SQL command and returns results as collection of values of specified type
        /// </summary>
        /// <param name="procedureName">Procedure name</param>
        /// <param name="parameters">Command parameters</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the entity entries
        /// </returns>
        public virtual async Task<IList<TEntity>> EntitiesFromSqlAsync(string procedureName, params DataParameter[] parameters)
        {
            return await _dataProvider.QueryAsync<TEntity>(procedureName, parameters?.ToArray());
        }

        /// <summary>
        /// Executes SQL using StoredProcedure command type and returns number of affected records
        /// </summary>
        /// <param name="procedureName">Procedure name</param>
        /// <param name="parameters">Command parameters</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the number of records, affected by command execution.
        /// </returns>
        public virtual async Task<int> ExecuteSpAsync(string procedureName, params DataParameter[] parameters)
        {
            return await _dataProvider.ExecuteStoredProcedureAsync(procedureName, parameters?.ToArray());
        }

        /// <summary>
        /// Executes SQL using StoredProcedure command type and returns results as collection of values of specified type
        /// </summary>
        /// <param name="procedureName">Procedure name</param>
        /// <param name="parameters">Command parameters</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the entity entry
        /// </returns>
        public virtual async Task<TEntity> EntityFromSpAsync(string procedureName, params DataParameter[] parameters)
        {
            return await _dataProvider.ExecuteStoredProcedureAsync<TEntity>(procedureName, parameters?.ToArray());
        }

        /// <summary>
        /// Executes SQL using StoredProcedure command type and returns results as collection of values of specified type
        /// </summary>
        /// <param name="procedureName">Procedure name</param>
        /// <param name="parameters">Command parameters</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the entity entries
        /// </returns>
        public virtual async Task<IList<TEntity>> EntitiesFromSpAsync(string procedureName, params DataParameter[] parameters)
        {
            return await _dataProvider.QueryProcAsync<TEntity>(procedureName, parameters?.ToArray());
        }

        /// <summary>
        /// Truncates database table
        /// </summary>
        /// <param name="resetIdentity">Performs reset identity column</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task TruncateAsync(bool resetIdentity = false)
        {
            await (await _dataProvider.GetTableAsync<TEntity>()).TruncateAsync(resetIdentity);
        }

        #endregion

        #region Log Methods

        /// <summary>
        /// Get paged list of all log entity entries
        /// </summary>
        /// <param name="func">Function to select entries</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="getOnlyTotalCount">Whether to get only the total number of entries without actually loading data</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the paged list of entity entries
        /// </returns>
        public virtual async Task<IPagedList<TEntity>> GetAllLogPagedAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>> func = null,
            int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false)
        {
            var query = LogTable;

            query = func != null ? func(query) : query;

            return await query.ToPagedListAsync(pageIndex, pageSize, getOnlyTotalCount);
        }

        /// <summary>
        /// Get paged list of all log entity entries
        /// </summary>
        /// <param name="func">Function to select entries</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="getOnlyTotalCount">Whether to get only the total number of entries without actually loading data</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the paged list of entity entries
        /// </returns>
        public virtual async Task<IPagedList<TEntity>> GetAllLogPagedAsync(Func<IQueryable<TEntity>, Task<IQueryable<TEntity>>> func = null,
            int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false)
        {
            var query = LogTable;

            query = func != null ? await func(query) : query;

            return await query.ToPagedListAsync(pageIndex, pageSize, getOnlyTotalCount);
        }

        /// <summary>
        /// Insert entity
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Entity</returns>
        public virtual async Task<TEntity> InsertLogAsync(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return await _dataProvider.InsertLogEntityAsync(entity);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public virtual async Task InsertLogAsync(IList<TEntity> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            await _dataProvider.BulkInsertLogEntitiesAsync(entities);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a table
        /// </summary>
        public virtual IQueryable<TEntity> Table => _dataProvider.GetTable<TEntity>().With("NOLOCK");

        #endregion

        #region Log Properties

        /// <summary>
        /// Gets a log table
        /// </summary>
        public virtual IQueryable<TEntity> LogTable => _dataProvider.GetLogTable<TEntity>().With("NOLOCK");

        #endregion
    }

}
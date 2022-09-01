using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LinqToDB.Data;
using SkyCars.Core;
using SkyCars.Core.DomainEntity.Grid;

namespace SkyCars.Data
{
    /// <summary>
    /// Represents an entity repository
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public partial interface IRepository<TEntity> where TEntity : BaseEntity
    {
        #region Methods

        /// <summary>
        /// Get the entity entry
        /// </summary>
        /// <param name="id">Entity entry identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the entity entry
        /// </returns>
        Task<TEntity> GetByIdAsync(int? id);

        /// <summary>
        /// Get entity entries by identifiers
        /// </summary>
        /// <param name="ids">Entity entry identifiers</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the entity entries
        /// </returns>
        Task<IList<TEntity>> GetByIdsAsync(IList<int> ids);

        /// <summary>
        /// Get all entity entries
        /// </summary>
        /// <param name="func">Function to select entries</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the entity entries
        /// </returns>
        Task<IList<TEntity>> GetAllAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>> func = null);

        /// <summary>
        /// Get all entity entries
        /// </summary>
        /// <param name="func">Function to select entries</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the entity entries
        /// </returns>
        Task<IList<TEntity>> GetAllAsync(Func<IQueryable<TEntity>, Task<IQueryable<TEntity>>> func = null);

        /// <summary>
        /// Get all entity entries
        /// </summary>
        /// <param name="func">Function to select entries</param>
        /// <returns>Entity entries</returns>
        IList<TEntity> GetAll(Func<IQueryable<TEntity>, IQueryable<TEntity>> func = null);

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
        Task<IPagedList<TEntity>> GetAllPagedAsync(GridRequestModel objGrid,IQueryable<TEntity> query=null, Func<IQueryable<TEntity>, IQueryable<TEntity>> func = null);

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
        Task<IPagedList<TEntity>> GetAllPagedAsync(Func<IQueryable<TEntity>, Task<IQueryable<TEntity>>> func = null,
            int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false);

        /// <summary>
        /// Insert the entity entry
        /// </summary>
        /// <param name="entity">Entity entry</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task<TEntity> InsertAsync(TEntity entity);

        /// <summary>
        /// Insert entity entries
        /// </summary>
        /// <param name="entities">Entity entries</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertAsync(IList<TEntity> entities);

        /// <summary>
        /// Update the entity entry
        /// </summary>
        /// <param name="entity">Entity entry</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateAsync(TEntity entity);

        /// <summary>
        /// Update entity entries
        /// </summary>
        /// <param name="entities">Entity entries</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateAsync(IList<TEntity> entities);

        /// <summary>
        /// Delete the entity entry
        /// </summary>
        /// <param name="entity">Entity entry</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteAsync(TEntity entity);

        /// <summary>
        /// Delete entity entries
        /// </summary>
        /// <param name="entities">Entity entries</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteAsync(IList<TEntity> entities);

        /// <summary>
        /// Delete entity entries by the passed predicate
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the number of deleted records
        /// </returns>
        Task<int> DeleteAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Loads the original copy of the entity entry
        /// </summary>
        /// <param name="entity">Entity entry</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the copy of the passed entity entry
        /// </returns>
        Task<TEntity> LoadOriginalCopyAsync(TEntity entity);

        /// <summary>
        /// Executes SQL command and returns number of affected records
        /// </summary>
        /// <param name="procedureName">Procedure name</param>
        /// <param name="parameters">Command parameters</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the number of records, affected by command execution.
        /// </returns>
        Task<int> ExecuteSqlAsync(string procedureName, params DataParameter[] parameters);

        /// <summary>
        /// Executes SQL command and returns results as collection of values of specified type
        /// </summary>
        /// <param name="procedureName">Procedure name</param>
        /// <param name="parameters">Command parameters</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the entity entry
        /// </returns>
        Task<TEntity> EntityFromSqlAsync(string procedureName, params DataParameter[] parameters);

        /// <summary>
        /// Executes SQL command and returns results as collection of values of specified type
        /// </summary>
        /// <param name="procedureName">Procedure name</param>
        /// <param name="parameters">Command parameters</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the entity entries
        /// </returns>
        Task<IList<TEntity>> EntitiesFromSqlAsync(string procedureName, params DataParameter[] parameters);

        /// <summary>
        /// Executes SQL using StoredProcedure command type and returns number of affected records
        /// </summary>
        /// <param name="procedureName">Procedure name</param>
        /// <param name="parameters">Command parameters</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the number of records, affected by command execution.
        /// </returns>
        Task<int> ExecuteSpAsync(string procedureName, params DataParameter[] parameters);

        /// <summary>
        /// Executes SQL using StoredProcedure command type and returns results as collection of values of specified type
        /// </summary>
        /// <param name="procedureName">Procedure name</param>
        /// <param name="parameters">Command parameters</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the entity entry
        /// </returns>
        Task<TEntity> EntityFromSpAsync(string procedureName, params DataParameter[] parameters);

        /// <summary>
        /// Executes SQL using StoredProcedure command type and returns results as collection of values of specified type
        /// </summary>
        /// <param name="procedureName">Procedure name</param>
        /// <param name="parameters">Command parameters</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the entity entries
        /// </returns>
        Task<IList<TEntity>> EntitiesFromSpAsync(string procedureName, params DataParameter[] parameters);

        /// <summary>
        /// Truncates database table
        /// </summary>
        /// <param name="resetIdentity">Performs reset identity column</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task TruncateAsync(bool resetIdentity = false);

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
        Task<IPagedList<TEntity>> GetAllLogPagedAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>> func = null,
            int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false);

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
        Task<IPagedList<TEntity>> GetAllLogPagedAsync(Func<IQueryable<TEntity>, Task<IQueryable<TEntity>>> func = null,
            int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false);

        /// <summary>
        /// Insert entity
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Entity</returns>
        Task<TEntity> InsertLogAsync(TEntity entity);

        /// <summary>
        /// Insert entities
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        Task InsertLogAsync(IList<TEntity> entities);

        #endregion

        #region Properties

        /// <summary>
        /// Gets a table
        /// </summary>
        IQueryable<TEntity> Table { get; }

        #endregion

        #region Log Properties

        /// <summary>
        /// Gets a log table
        /// </summary>
        IQueryable<TEntity> LogTable { get; }

        #endregion
    }
}
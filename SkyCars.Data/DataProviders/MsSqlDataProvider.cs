using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider;
using LinqToDB.DataProvider.SqlServer;
using SkyCars.Core;

namespace SkyCars.Data.DataProviders
{
    /// <summary>
    /// Represents the MS SQL Server data provider
    /// </summary>
    public partial class MsSqlDataProvider : BaseDataProvider, IDataProvider
    {
        #region Fields

        private static readonly Lazy<LinqToDB.DataProvider.IDataProvider> _dataProvider = new(() => new SqlServerDataProvider(ProviderName.SqlServer, SqlServerVersion.v2012, SqlServerProvider.SystemDataSqlClient), true);

        #endregion

        #region Utils

        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task<SqlConnectionStringBuilder> GetConnectionStringBuilderAsync()
        {
            var connectionString = (await DataSettingsManager.LoadSettingsAsync()).ConnectionString;

            return new SqlConnectionStringBuilder(connectionString);
        }

        protected virtual SqlConnectionStringBuilder GetConnectionStringBuilder()
        {
            var connectionString = DataSettingsManager.LoadSettings().ConnectionString;

            return new SqlConnectionStringBuilder(connectionString);
        }

        #endregion

        #region Utils

        /// <summary>
        /// Gets a connection to the database for a current data provider
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        /// <returns>Connection to a database</returns>
        protected override DbConnection GetInternalDbConnection(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException(nameof(connectionString));

            return new SqlConnection(connectionString);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get the current identity value
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the integer identity; null if cannot get the result
        /// </returns>
        public virtual async Task<int?> GetTableIdentAsync<TEntity>() where TEntity : BaseEntity
        {
            using var currentConnection = await CreateDataConnectionAsync();
            var tableName = GetEntityDescriptor<TEntity>().TableName;

            var result = currentConnection.Query<decimal?>($"SELECT IDENT_CURRENT('[{tableName}]') as Value")
                .FirstOrDefault();

            return result.HasValue ? Convert.ToInt32(result) : 1;
        }

        /// <summary>
        /// Set table identity (is supported)
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="ident">Identity value</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task SetTableIdentAsync<TEntity>(int ident) where TEntity : BaseEntity
        {
            using var currentConnection = await CreateDataConnectionAsync();
            var currentIdent = await GetTableIdentAsync<TEntity>();
            if (!currentIdent.HasValue || ident <= currentIdent.Value)
                return;

            var tableName = GetEntityDescriptor<TEntity>().TableName;

            await currentConnection.ExecuteAsync($"DBCC CHECKIDENT([{tableName}], RESEED, {ident})");
        }

        /// <summary>
        /// Re-index database tables
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task ReIndexTablesAsync()
        {
            using var currentConnection = await CreateDataConnectionAsync();
            var commandText = $@"
                    DECLARE @TableName sysname 
                    DECLARE cur_reindex CURSOR FOR
                    SELECT table_name
                    FROM [{currentConnection.Connection.Database}].information_schema.tables
                    WHERE table_type = 'base table'
                    OPEN cur_reindex
                    FETCH NEXT FROM cur_reindex INTO @TableName
                    WHILE @@FETCH_STATUS = 0
                        BEGIN
                            exec('ALTER INDEX ALL ON [' + @TableName + '] REBUILD')
                            FETCH NEXT FROM cur_reindex INTO @TableName
                        END
                    CLOSE cur_reindex
                    DEALLOCATE cur_reindex";

            await currentConnection.ExecuteAsync(commandText);
        }

        /// <summary>
        /// Gets the name of an index
        /// </summary>
        /// <param name="targetTable">Target table name</param>
        /// <param name="targetColumn">Target column name</param>
        /// <returns>Name of an index</returns>
        public virtual string GetIndexName(string targetTable, string targetColumn)
        {
            return $"IX_{targetTable}_{targetColumn}";
        }

        /// <summary>
        /// Updates records in table, using values from entity parameter. 
        /// Records to update are identified by match on primary key value from obj value.
        /// </summary>
        /// <param name="entities">Entities with data to update</param>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task BulkUpdateEntitiesAsync<TEntity>(IEnumerable<TEntity> entities)
        {
            using var dataContext = await CreateDataConnectionAsync();
            await dataContext.GetTable<TEntity>()
                .Merge()
                .Using(entities)
                .OnTargetKey()
                .UpdateWhenMatched()
                .MergeAsync();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Sql server data provider
        /// </summary>
        protected override LinqToDB.DataProvider.IDataProvider LinqToDbDataProvider => _dataProvider.Value;

        /// <summary>
        /// Gets allowed a limit input value of the data for hashing functions, returns 0 if not limited
        /// </summary>
        public int SupportedLengthOfBinaryHash { get; } = 8000;

        /// <summary>
        /// Gets a value indicating whether this data provider supports backup
        /// </summary>
        public virtual bool BackupSupported => true;

        #endregion
    }
}

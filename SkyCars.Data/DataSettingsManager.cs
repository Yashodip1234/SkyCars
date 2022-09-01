using System;
using System.Linq;
using System.Threading.Tasks;
using FluentMigrator;
using Microsoft.Extensions.DependencyInjection;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using SkyCars.Data.DataProviders;
using SkyCars.Core.Infrastruct;

namespace SkyCars.Data
{
    /// <summary>
    /// Represents the database settings manager
    /// </summary>
    public static class DataSettingsManager
    {
        #region Methods

        /// <summary>
        /// Load database settings
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the data settings
        /// </returns>
        public static async Task<DataSettings> LoadSettingsAsync()
        {
            if (Singleton<DataSettings>.Instance != null)
                return Singleton<DataSettings>.Instance;

            /*
            //Need logic to read again from appsetting.json
            var text = fileProvider.ReadAllText(filePath, Encoding.UTF8);
            if (string.IsNullOrEmpty(text))
                return new DataSettings();

            //get data settings from the JSON file
            Singleton<DataSettings>.Instance = JsonConvert.DeserializeObject<DataSettings>(text);

            return Singleton<DataSettings>.Instance;*/

            return null;
        }

        /// <summary>
        /// Load database settings
        /// </summary>
        /// <returns>Data settings</returns>
        public static DataSettings LoadSettings()
        {
            if (Singleton<DataSettings>.Instance != null)
                return Singleton<DataSettings>.Instance;

            /*
            //Need logic to read again from appsetting.json
            var text = fileProvider.ReadAllText(filePath, Encoding.UTF8);
            if (string.IsNullOrEmpty(text))
                return new DataSettings();

            //get data settings from the JSON file
            Singleton<DataSettings>.Instance = JsonConvert.DeserializeObject<DataSettings>(text);

            return Singleton<DataSettings>.Instance;*/

            return null;
        }

        /// <summary>
        /// Initialize database settings
        /// </summary>
        public static void IntiDatabaseSettings(IServiceCollection services, DataSettings DBSettings)
        {
            SetSettings(DBSettings);

            //Set up the fluent migrator for the dependency injector
            services.AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
            .AddSqlServer()
            // Set the connection string
            .WithGlobalConnectionString(DBSettings.ConnectionString)
            // Define the assembly containing the migrations
            .ScanIn(typeof(DataSettings).Assembly).For.Migrations());
        }

        /// <summary>
        /// Executes all found (and unapplied) migrations
        /// </summary>
        public static void ApplyUpMigrations(IServiceProvider services)
        {
            IFilteringMigrationSource _filteringMigrationSource = services.GetService<IFilteringMigrationSource>();
            IMigrationRunner _migrationRunner = services.GetService<IMigrationRunner>();
            IMigrationRunnerConventions _migrationRunnerConventions = services.GetService<IMigrationRunnerConventions>();

            var migrations = _filteringMigrationSource.GetMigrations(null) ?? Enumerable.Empty<IMigration>();

            var sortedMigrations = migrations.Select(m => _migrationRunnerConventions.GetMigrationInfoForMigration(m)).OrderBy(migration => migration.Version);

            foreach (var migrationInfo in sortedMigrations)
            {
                /*if (migrationInfo.Migration.GetType().GetCustomAttributes(typeof(SkipMigrationOnUpdateAttribute)).Any())
                    continue;*/

                _migrationRunner.MigrateUp(migrationInfo.Version);
            }
        }

        /// <summary>
        /// Set database settings
        /// </summary>
        /// <param name="connectionString">Primary database settings</param>
        /// <param name="logConnectionString">Log database settings</param>
        public static void SetSettings(DataSettings DBSettings)
        {
            var dataSettings = new DataSettings
            {
                ConnectionString = DBSettings.ConnectionString,
                LogConnectionString = DBSettings.LogConnectionString
            };

            Singleton<DataSettings>.Instance = dataSettings;
        }

        /// <summary>
        /// Gets the command execution timeout.
        /// </summary>
        /// <value>
        /// Number of seconds. Negative timeout value means that a default timeout will be used. 0 timeout value corresponds to infinite timeout.
        /// </value>
        /// <returns>A task that represents the asynchronous operation</returns>
        public static async Task<int> GetSqlCommandTimeoutAsync()
        {
            return (await LoadSettingsAsync())?.SQLCommandTimeout ?? -1;
        }

        /// <summary>
        /// Gets the command execution timeout.
        /// </summary>
        /// <value>
        /// Number of seconds. Negative timeout value means that a default timeout will be used. 0 timeout value corresponds to infinite timeout.
        /// </value>
        public static int GetSqlCommandTimeout()
        {
            return (LoadSettings())?.SQLCommandTimeout ?? -1;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets data provider
        /// </summary>
        public static IDataProvider DataProvider
        {
            get
            {
                return new MsSqlDataProvider(); //MySqlDataProvider()
            }
        }

        #endregion
    }

    /// <summary>
    /// Represents the database settings
    /// </summary>
    public partial class DataSettings
    {
        #region Properties

        /// <summary>
        /// Gets or sets a connection string
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets a log connection string
        /// </summary>
        public string LogConnectionString { get; set; }

        /*/// <summary>
        /// Gets or sets a database provider
        /// </summary>
        public DataProviderType DataProvider { get; set; }*/

        /// <summary>
        /// Gets or sets the wait time (in seconds) before terminating the attempt to execute a command and generating an error.
        /// By default, timeout isn't set and a default value for the current provider used. 
        /// Set 0 to use infinite timeout.
        /// </summary>
        public int? SQLCommandTimeout { get; set; }

        #endregion
    }
}
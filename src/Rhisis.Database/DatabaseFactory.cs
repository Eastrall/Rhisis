using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using Rhisis.Core.Structures.Configuration;
using System;

namespace Rhisis.Database
{
    /// <summary>
    /// Provides methods to instanciate new database connections.
    /// </summary>
    public class DatabaseFactory : IDesignTimeDbContextFactory<RhisisDatabaseContext>
    {
        private const string MigrationConfigurationEnv = "DB_CONFIG";
        
        /// <inheritdoc />
        public RhisisDatabaseContext CreateDbContext(string[] args)
        {
            string configurationPath = Environment.GetEnvironmentVariable(MigrationConfigurationEnv);
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile(configurationPath, optional: false)
                .Build();
            DatabaseConfiguration dbConfiguration = configuration.GetSection(ConfigurationConstants.DatabaseConfiguration).Get<DatabaseConfiguration>();

            if (dbConfiguration == null)
            {
                throw new InvalidOperationException($"Cannot find database configuration path: '{configurationPath}'.");
            }

            return CreateDatabaseInstance(dbConfiguration) as RhisisDatabaseContext;
        }
        
        /// <summary>
        /// Creates a database on the given database configuration.
        /// Lifetime should be managed by IOC framework if using any!
        /// </summary>
        /// <param name="configuration">The database configuration to use</param>
        /// <returns>A new instance of an IDatabase implementation</returns>
        public IRhisisDatabase CreateDatabaseInstance(DatabaseConfiguration configuration)
        {
            var optionsBuilder = new DbContextOptionsBuilder().UseMySql(
                BuildConnectionString(configuration), 
                new MySqlServerVersion(configuration.ServerVersion));

            return new RhisisDatabaseContext(optionsBuilder.Options, configuration);
        }

        /// <summary>
        /// Builds the MySQL database connection string.
        /// </summary>
        /// <param name="databaseConfiguration">Database configuration.</param>
        /// <returns>MySQL connection string.</returns>
        public static string BuildConnectionString(DatabaseConfiguration databaseConfiguration)
        {
            var sqlConnectionStringBuilder = new MySqlConnectionStringBuilder
            {
                Server = databaseConfiguration.Host,
                UserID = databaseConfiguration.Username,
                Password = databaseConfiguration.Password,
                Port = (uint)databaseConfiguration.Port,
                Database = databaseConfiguration.Database,
                SslMode = MySqlSslMode.None,
                AllowPublicKeyRetrieval = databaseConfiguration.AllowPublicKeyRetrieval,
                //ServerRsaPublicKeyFile = databaseConfiguration.ServerRSAPublicKeyFile
            };

            return sqlConnectionStringBuilder.ToString();
        }
    }
}
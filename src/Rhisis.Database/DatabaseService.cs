using Rhisis.Database.Exceptions;

namespace Rhisis.Database
{
    public static class DatabaseService
    {
        internal static DatabaseConfiguration Configuration { get; private set; }

        /// <summary>
        /// Configure the database access.
        /// </summary>
        /// <param name="configuration"></param>
        public static void Configure(DatabaseConfiguration configuration)
        {
            Configure(
                configuration.Host, 
                configuration.Port, 
                configuration.Username, 
                configuration.Password, 
                configuration.Database, 
                configuration.Provider);
        }

        /// <summary>
        /// Configure the database access.
        /// </summary>
        /// <param name="host">Database remote host</param>
        /// <param name="port">Database remote port</param>
        /// <param name="username">Database username</param>
        /// <param name="password">Database password</param>
        /// <param name="databaseName">Database name</param>
        /// <param name="provider">Database provider</param>
        public static void Configure(string host, int port, string username, string password, string databaseName, DatabaseProvider provider)
        {
            if (Configuration != null)
                throw new RhisisDatabaseConfigurationException("Database is already configured");

            Configuration = new DatabaseConfiguration
            {
                Host = host,
                Port = port,
                Username = username,
                Password = password,
                Database = databaseName,
                Provider = provider
            };
        }

        /// <summary>
        /// Unload the database access configuration.
        /// </summary>
        public static void UnloadConfiguration()
        {
            Configuration = null;
        }

        /// <summary>
        /// Gets an access to the database.
        /// </summary>
        /// <returns></returns>
        public static DatabaseContext GetContext()
        {
            return Configuration == null
                ? throw new RhisisDatabaseConfigurationException("Database configuration is not set.")
                : new DatabaseContext(Configuration);
        }
    }
}

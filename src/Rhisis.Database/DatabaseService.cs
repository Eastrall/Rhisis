using Rhisis.Database.Contexts;
using Rhisis.Database.Exceptions;
using System;
using Rhisis.Core.Structures.Configuration;

namespace Rhisis.Database
{
    public static class DatabaseService
    {
        internal static DatabaseConfiguration Configuration { get; private set; }

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

        public static void Configure(string host, int port, string username, string password, string databaseName, DatabaseProvider provider)
        {
            if (Configuration != null)
                throw new RhisisDatabaseConfigurationException("Database is already configured");

            Configuration = new DatabaseConfiguration()
            {
                Host = host,
                Port = port,
                Username = username,
                Password = password,
                Database = databaseName,
                Provider = provider
            };
        }

        public static void UnloadConfiguration()
        {
            Configuration = null;
        }

        public static DatabaseContext GetContext()
        {
            if (Configuration == null)
                throw new RhisisDatabaseConfigurationException("Database is not configured.");

            switch (Configuration.Provider)
            {
                case DatabaseProvider.MsSQL:
                    return new MsSQLContext(Configuration);

                case DatabaseProvider.SQLite:
                case DatabaseProvider.PostgreSQL:
                    throw new NotImplementedException();

                case DatabaseProvider.MySQL:
                default:
                    return new MySQLContext(Configuration);
            }
        }
    }
}

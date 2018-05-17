using Microsoft.EntityFrameworkCore;
using Rhisis.Core.Structures.Configuration;

namespace Rhisis.Database.Contexts
{
    /// <summary>
    /// Represents the MySQL Server database context.
    /// </summary>
    internal class MySQLContext : DatabaseContext
    {
        /// <summary>
        /// MySQL server connection string.
        /// </summary>
        private static readonly string MySQLConnectionString = "server={0};userid={1};pwd={2};port={4};database={3};sslmode=none;";

        /// <summary>
        /// Creates a new <see cref="MySQLContext"/> instance.
        /// </summary>
        /// <param name="configuration">Database configuration</param>
        public MySQLContext(DatabaseConfiguration configuration) 
            : base(configuration)
        {
        }

        /// <summary>
        /// Called when the configuration of the DbContext has begun.
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = string.Format(MySQLConnectionString,
                this.Configuration.Host,
                this.Configuration.Username,
                this.Configuration.Password,
                this.Configuration.Database,
                this.Configuration.Port);

            optionsBuilder.UseMySql(connectionString);
            base.OnConfiguring(optionsBuilder);
        }
    }
}

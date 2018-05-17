using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Rhisis.Core.Structures.Configuration;

namespace Rhisis.Database.Contexts
{
    /// <summary>
    /// Represents the Microsoft SQL Server database context.
    /// </summary>
    internal class MsSQLContext : DatabaseContext
    {
        /// <summary>
        /// Microsoft SQL Server connection string.
        /// </summary>
        private static readonly string MsSQLConnectionString = "Server={0};Database={1};User Id={2};Password={3};";

        /// <summary>
        /// Creates a new <see cref="MsSQLContext"/> instance.
        /// </summary>
        /// <param name="configuration">Database configuration</param>
        public MsSQLContext(DatabaseConfiguration configuration) 
            : base(configuration)
        {
        }

        /// <summary>
        /// Called when the configuration of the DbContext has begun.
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = string.Format(MsSQLConnectionString,
               this.Configuration.Host,
               this.Configuration.Database,
               this.Configuration.Username,
               this.Configuration.Password);

            optionsBuilder.UseSqlServer(connectionString);
            base.OnConfiguring(optionsBuilder);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Rhisis.Core.Helpers;
using Rhisis.Database.Entities;
using System;
using System.Threading.Tasks;

namespace Rhisis.Database.Context
{
    public sealed class DatabaseContext : DbContext
    {
        private const string MigrationConfigurationEnv = "DB_CONFIG";
        private const string MySqlConnectionString = "server={0};userid={1};pwd={2};port={4};database={3};sslmode=none;";
        private const string MsSqlConnectionString = "Server={0},{1};Database={2};User Id={3};Password={4};";

        private readonly DatabaseConfiguration _configuration;

        /// <summary>
        /// Gets or sets the users.
        /// </summary>
        public DbSet<DbUser> Users { get; set; }

        /// <summary>
        /// Gets or sets the characters.
        /// </summary>
        public DbSet<DbCharacter> Characters { get; set; }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        public DbSet<DbItem> Items { get; set; }

        /// <summary>
        /// Gets or sets the mails.
        /// </summary>
        public DbSet<DbMail> Mails { get; set; }

        /// <summary>
        /// Create a new <see cref="DatabaseContext"/> instance.
        /// </summary>
        /// <param name="options"></param>
        public DatabaseContext(DbContextOptions options)
            : base(options)
        {
        }

        /// <summary>
        /// Creates a new <see cref="DatabaseContext"/> instance.
        /// </summary>
        /// <param name="configuration"></param>
        public DatabaseContext(DatabaseConfiguration configuration)
        {
            this._configuration = configuration;
        }

        /// <inheritdoc />
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured)
                return;

            switch (this._configuration.Provider)
            {
                case DatabaseProvider.MySql:
                    optionsBuilder.UseMySql(
                        string.Format(MySqlConnectionString,
                            this._configuration.Host,
                            this._configuration.Username,
                            this._configuration.Password,
                            this._configuration.Database,
                            this._configuration.Port));
                    break;

                case DatabaseProvider.MsSql:
                    optionsBuilder.UseSqlServer(
                        string.Format(MsSqlConnectionString,
                            this._configuration.Host,
                            this._configuration.Port,
                            this._configuration.Database,
                            this._configuration.Username,
                            this._configuration.Password));
                    break;

                default: throw new NotImplementedException($"Provider {this._configuration.Provider} not implemented yet.");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<DbCharacter>().HasMany(x => x.Mails).WithOne(x => x.Receiver);
            modelBuilder.Entity<DbMail>().HasKey(x => x.Id);
            modelBuilder.Entity<DbMail>()
                .HasOne(x => x.Receiver).WithMany(x => x.Mails);
        }

        /// <summary>
        /// Migrates the database schema.
        /// </summary>
        public void Migrate() => this.Database.Migrate();

        /// <summary>
        /// Migrates the database schema asynchronously.
        /// </summary>
        public async Task MigrateAsync() => await this.Database.MigrateAsync();

        /// <summary>
        /// Check if the database exists.
        /// </summary>
        /// <returns></returns>
        public bool DatabaseExists() => (this.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).Exists();

        /// <summary>
        /// Opens the connection.
        /// </summary>
        public void OpenConnection() => this.Database.OpenConnection();

        /// <summary>
        /// Closes the connection.
        /// </summary>
        public void CloseConnection() => this.Database.CloseConnection();
    }
}

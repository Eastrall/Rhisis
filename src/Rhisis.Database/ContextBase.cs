using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Rhisis.Core.Structures.Configuration;
using Rhisis.Database.Interfaces;
using Rhisis.Database.Repositories;
using Rhisis.Database.Structures;

namespace Rhisis.Database
{
    public abstract class DatabaseContext : DbContext
    {
        /// <summary>
        /// Gets the <see cref="User"/> repository.
        /// </summary>
        public IRepository<User> Users { get; }

        /// <summary>
        /// Gets the <see cref="Character"/> repository.
        /// </summary>
        public IRepository<Character> Characters { get; }

        /// <summary>
        /// Gets the <see cref="Item"/> repository.
        /// </summary>
        public IRepository<Item> Items { get; }

        /// <summary>
        /// Gets the <see cref="DatabaseContext"/> configuration.
        /// </summary>
        protected DatabaseConfiguration Configuration { get; private set; }

        internal DbSet<User> UsersDbSet { get; set; }

        internal DbSet<Character> CharactersDbSet { get; set; }

        internal DbSet<Item> ItemsDbSet { get; set; }

        /// <summary>
        /// Creates a new <see cref="DatabaseContext"/> instance.
        /// </summary>
        /// <param name="configuration"></param>
        protected DatabaseContext(DatabaseConfiguration configuration)
        {
            this.Users = new UserRepository(this);
            this.Characters = new CharacterRepository(this);
            this.Items = new ItemRepository(this);

            this.Configuration = configuration;
        }

        /// <summary>
        /// Called when the configuration of the DbContext has begun.
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        /// <summary>
        /// Opens the connection.
        /// </summary>
        public virtual void OpenConnection() => this.Database.OpenConnection();

        /// <summary>
        /// Closes the connection.
        /// </summary>
        public virtual void CloseConnection() => this.Database.CloseConnection();

        /// <summary>
        /// Creates the database.
        /// </summary>
        /// <returns></returns>
        public virtual bool CreateDatabase() => this.Database.EnsureCreated();

        /// <summary>
        /// Check if the database exists.
        /// </summary>
        /// <returns></returns>
        public virtual bool DatabaseExists()
        {
            return (this.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).Exists();
        }

        /// <summary>
        /// Processes the database migration.
        /// </summary>
        public virtual void Migrate()
        {
            try
            {
                var databaseCreator = this.Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;
                databaseCreator.CreateTables();
            }
            catch
            {
                // Nothing to do.
            }
            finally
            {
                this.Database.Migrate();
            }
        }
    }
}

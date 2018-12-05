using Microsoft.EntityFrameworkCore;
using Rhisis.Database.Context;
using Rhisis.Database.Repositories;
using Rhisis.Database.Repositories.Implementation;
using System.Threading.Tasks;

namespace Rhisis.Database
{
    public sealed class Database : IDatabase
    {
        private readonly DatabaseContext _databaseContext;

        /// <inheritdoc />
        public IUserRepository Users { get; private set; }

        /// <inheritdoc />
        public ICharacterRepository Characters { get; private set; }

        /// <inheritdoc />
        public IItemRepository Items { get; private set; }

        public IMailRepository Mails { get; private set; }

        /// <summary>
        /// Creates a new <see cref="Database"/> object instance.
        /// </summary>
        public Database()
        {
            this._databaseContext = DatabaseFactory.Instance.CreateDbContext();
            this.InitializeRepositories();
        }

        /// <summary>
        /// Creates a new <see cref="Database"/> with EF context options.
        /// </summary>
        /// <param name="options"></param>
        public Database(DbContextOptions options)
        {
            this._databaseContext = DatabaseFactory.Instance.CreateDbContext(options);
            this.InitializeRepositories();
        }

        /// <inheritdoc />
        public void Complete() => this._databaseContext.SaveChanges();

        /// <inheritdoc />
        public async Task CompleteAsync() => await this._databaseContext.SaveChangesAsync();

        /// <inheritdoc />
        public void Dispose() => this._databaseContext.Dispose();

        /// <summary>
        /// Initializes the repositories.
        /// </summary>
        private void InitializeRepositories()
        {
            this.Users = new UserRepository(this._databaseContext);
            this.Characters = new CharacterRepository(this._databaseContext);
            this.Items = new ItemRepository(this._databaseContext);
            this.Mails = new MailRepository(this._databaseContext);
        }
    }
}

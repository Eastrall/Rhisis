using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DataEncryption;
using Microsoft.EntityFrameworkCore.DataEncryption.Providers;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Rhisis.Database.Entities;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using Rhisis.Core.Structures.Configuration;

namespace Rhisis.Database
{
    public class RhisisDatabaseContext : DbContext, IRhisisDatabase
    {
        private readonly IEncryptionProvider _encryptionProvider;

        /// <inheritdoc />
        public DbSet<DbUser> Users { get; set; }

        /// <inheritdoc />
        public DbSet<DbCharacter> Characters { get; set; }

        /// <inheritdoc />
        public DbSet<DbItem> Items { get; set; }

        /// <inheritdoc />
        public DbSet<DbInventoryItem> InventoryItems { get; set; }

        /// <inheritdoc />
        public DbSet<DbMail> Mails { get; set; }

        /// <inheritdoc />
        public DbSet<DbShortcut> TaskbarShortcuts { get; set; }

        /// <inheritdoc />
        public DbSet<DbQuest> Quests { get; set; }

        /// <inheritdoc />
        public DbSet<DbSkill> Skills { get; set; }

        /// <inheritdoc />
        public DbSet<DbAttribute> Attributes { get; set; }

        /// <inheritdoc />
        public DbSet<DbItemAttributes> ItemAttributes { get; set; }

        /// <summary>
        /// Create a new <see cref="RhisisDatabaseContext"/> instance.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="configuration"></param>
        public RhisisDatabaseContext(DbContextOptions options, DatabaseConfiguration configuration = null)
            : base(options)
        {
            if (configuration != null)
            {
                if (string.IsNullOrEmpty(configuration.EncryptionKey))
                {
                    return;
                }

                if (configuration.UseEncryption)
                {
                    _encryptionProvider = new AesProvider(
                        Convert.FromBase64String(configuration.EncryptionKey),
                        Enumerable.Repeat<byte>(0, 16).ToArray(),
                        CipherMode.CBC,
                        PaddingMode.Zeros);
                }
            }
        }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            if (_encryptionProvider != null)
            {
                modelBuilder.UseEncryption(_encryptionProvider);
            }

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        /// <inheritdoc />
        public void Migrate() => Database.Migrate();

        /// <inheritdoc />
        public bool Exists() => (this.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).Exists();

        /// <inheritdoc />
        public bool IsAlive()
        {
            try
            {
                Database.OpenConnection();
                Database.CloseConnection();
            }
            catch (SqlException)
            {
                return false;
            }

            return true;
        }
    }
}

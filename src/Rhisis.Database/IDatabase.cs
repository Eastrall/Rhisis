using Rhisis.Database.Repositories;
using System;
using System.Threading.Tasks;

namespace Rhisis.Database
{
    public interface IDatabase : IDisposable
    {
        /// <summary>
        /// Gets the users.
        /// </summary>
        IUserRepository Users { get; }

        /// <summary>
        /// Gets the characters.
        /// </summary>
        ICharacterRepository Characters { get; }

        /// <summary>
        /// Gets the items.
        /// </summary>
        IItemRepository Items { get; }

        /// <summary>
        /// Gets the mails.
        /// </summary>
        IMailRepository Mails { get; }

        /// <summary>
        /// Complete the pending database operations.
        /// </summary>
        void Complete();

        /// <summary>
        /// Complete the pending database operations in an asynchronous context.
        /// </summary>
        /// <returns></returns>
        Task CompleteAsync();
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Rhisis.Database.Entities
{
    public class DbUser : DbEntity
    {
        /// <summary>
        /// Gets or sets the user's identification name.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the user's password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the user's email address.
        /// </summary>
        [Encrypted]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets a boolean value that indicates that the user email address has been confirmed.
        /// </summary>
        public bool EmailConfirmed { get; set; }

        /// <summary>
        /// Gets or sets the user's creation date.
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        /// <summary>
        /// Gets or sets the last time the user logged in.
        /// </summary>
        public DateTime LastConnectionTime { get; set; }
        
        /// <summary>
        /// Gets or sets the user's authority.
        /// </summary>
        public int Authority { get; set; }

        /// <summary>
        /// Gets or sets a flag that indicates if the user is deleted.
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Gets the total play time of this user in seconds.
        /// </summary>
        public long PlayTime => Characters.Sum(x => x.PlayTime);

        /// <summary>
        /// Gets or sets the user's characters list.
        /// </summary>
        public ICollection<DbCharacter> Characters { get; set; }

        /// <summary>
        /// Creates a new <see cref="DbUser"/> instance.
        /// </summary>
        public DbUser()
        {
            CreatedAt = DateTime.UtcNow;
            Characters = new HashSet<DbCharacter>();
        }
    }
}

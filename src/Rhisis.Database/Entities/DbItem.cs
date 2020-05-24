using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Rhisis.Database.Entities
{
    [DebuggerDisplay("Id: {ItemId}")]
    public sealed class DbItem : DbEntity
    {        
        /// <summary>
        /// Gets or sets the real item Id.
        /// </summary>
        public int ItemId { get; set; }
        
        /// <summary>
        /// Gets or sets the item creator id.
        /// </summary>
        public int CreatorId { get; set; }
        
        /// <summary>
        /// Gets or sets the item refine.
        /// </summary>
        public byte? Refine { get; set; }

        /// <summary>
        /// Gets or sets the item element.
        /// </summary>
        public byte? Element { get; set; }

        /// <summary>
        /// Gets or sets the item element refine.
        /// </summary>
        public byte? ElementRefine { get; set; }

        /// <summary>
        /// Gets or sets a flag that indicates if the item is deleted.
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Gets the item attributes.
        /// </summary>
        public ICollection<DbItemAttributes> ItemAttributes { get; set; }
    }
}

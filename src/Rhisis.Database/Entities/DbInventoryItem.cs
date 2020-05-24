namespace Rhisis.Database.Entities
{
    public class DbInventoryItem : DbEntity
    {
        /// <summary>
        /// Gets or sets the character id.
        /// </summary>
        public int CharacterId { get; set; }

        /// <summary>
        /// Gets or sets the character instance.
        /// </summary>
        public DbCharacter Character { get; set; }

        /// <summary>
        /// Gets or sets the item id.
        /// </summary>
        public int ItemId { get; set; }

        /// <summary>
        /// Gets or sets the item instance.
        /// </summary>
        public DbItem Item { get; set; }

        /// <summary>
        /// Gets or sets the item inventory slot number.
        /// </summary>
        public int Slot { get; set; }

        /// <summary>
        /// Gets or sets the item inventory quantity.
        /// </summary>
        public int Quantity { get; set; }
    }
}

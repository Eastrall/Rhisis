namespace Rhisis.Database.Entities
{
    /// <summary>
    /// Provides a many-to-many relashioship between a <see cref="DbItem"/> and a <see cref="DbAttribute"/>.
    /// </summary>
    public class DbItemAttributes
    {
        /// <summary>
        /// Gets or sets the item id.
        /// </summary>
        public int ItemId { get; set; }

        /// <summary>
        /// Gets or sets the item instance.
        /// </summary>
        public DbItem Item { get; set; }

        /// <summary>
        /// Gets or sets the attribute id.
        /// </summary>
        public int AttributeId { get; set; }

        /// <summary>
        /// Gets or sets the attribute instance.
        /// </summary>
        public DbAttribute Attribute { get; set; }

        /// <summary>
        /// Gets or sets the item attribute value.
        /// </summary>
        public int Value { get; set; }
    }
}

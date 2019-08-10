using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Rhisis.Core.Structures.Game
{
    /// <summary>
    /// Represents a NPC Shop data.
    /// </summary>
    [DataContract]
    public class ShopData
    {
        /// <summary>
        /// Gets the default shop tab count.
        /// </summary>
        public const int DefaultTabCount = 4;

        /// <summary>
        /// Gets or sets the shop name.
        /// </summary>
        [DataMember(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the shop items.
        /// </summary>
        [DataMember(Name = "items")]
        public List<ItemDescriptor>[] Items { get; set; }

        /// <summary>
        /// Creates a new <see cref="ShopData"/> instance.
        /// </summary>
        /// <param name="shopName">Npc shop name</param>
        /// <param name="shopTabs">Npc shop tabs data</param>
        public ShopData(string shopName, int shopTabs = DefaultTabCount)
        {
            this.Name = shopName;
            this.Items = new List<ItemDescriptor>[shopTabs];

            for (var i = 0; i < shopTabs; i++)
            {
                this.Items[i] = new List<ItemDescriptor>();
            }
        }
    }
}

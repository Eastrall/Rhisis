using Rhisis.Core.Data;
using System.Runtime.Serialization;

namespace Rhisis.Core.Structures.Game
{
    [DataContract]
    public class ItemBase
    {
        /// <summary>
        /// Gets the item Id.
        /// </summary>
        [DataMember(Name = "itemId")]
        public int Id { get; protected set; }

        /// <summary>
        /// Gets or sets the item refine.
        /// </summary>
        [DataMember(Name = "refine")]
        public byte Refine { get; set; }

        /// <summary>
        /// Gets or sets the item element. (Fire, water, electricity, etc...)
        /// </summary>
        [DataMember(Name = "element")]
        public ElementType Element { get; set; }

        /// <summary>
        /// Gets or sets the item element refine.
        /// </summary>
        [DataMember(Name = "elementRefine")]
        public byte ElementRefine { get; set; }

        /// <summary>
        /// Gets the items refine options.
        /// </summary>
        [IgnoreDataMember]
        public int Refines => Refine & (byte)Element & ElementRefine;

        /// <summary>
        /// Gets the item data informations.
        /// </summary>
        [IgnoreDataMember]
        public ItemData Data { get; protected set; }

        /// <summary>
        /// Creates a new <see cref="ItemBase"/> instance.
        /// </summary>
        protected ItemBase()
        {
        }

        public override string ToString() => $"{Data?.Name ?? "[undefined]"}";
    }
}

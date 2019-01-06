using System.Runtime.Serialization;

namespace Rhisis.Core.Structures.Configuration
{
    [DataContract]
    public class WorldDrops
    {
        public const int DefaultOwnershipTime = 7;
        public const int DefaultDespawnTime = 120;

        /// <summary>
        /// Gets or sets the drop Ownership time expressed in seconds.
        /// </summary>
        [DataMember(Name = "ownershipTime")]
        public int OwnershipTime { get; set; }

        /// <summary>
        /// Gets or sets the drop despawn time expressed in seconds.
        /// </summary>
        [DataMember(Name = "despawnTime")]
        public int DespawnTime { get; set; }

        /// <summary>
        /// Creates a new <see cref="WorldDrops"/> instance.
        /// </summary>
        public WorldDrops()
        {
            this.OwnershipTime = DefaultOwnershipTime;
            this.DespawnTime = DefaultDespawnTime;
        }
    }
}

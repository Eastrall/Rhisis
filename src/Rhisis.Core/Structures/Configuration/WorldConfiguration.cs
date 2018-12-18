using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Rhisis.Core.Structures.Configuration
{
    /// <summary>
    /// Represents the World Server configuration structure.
    /// </summary>
    [DataContract]
    public class WorldConfiguration : BaseConfiguration
    {
        public const string DefaultLanguage = "en";
        public const int DefaultMailShippingCost = 500;

        /// <summary>
        /// Gets or sets the world's id.
        /// </summary>
        [DataMember(Name = "id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the parent cluster server id.
        /// </summary>
        [DataMember(Name = "clusterId")]
        public int ClusterId { get; set; }

        /// <summary>
        /// Gets or sets the world's name.
        /// </summary>
        [DataMember(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the enabled or disabled systems of the world server.
        /// </summary>
        [DataMember(Name = "systems")]
        public IDictionary<string, bool> Systems { get; set; }

        /// <summary>
        /// Gets or sets the maps of the world server.
        /// </summary>
        [DataMember(Name = "maps")]
        public IEnumerable<string> Maps { get; set; }

        /// <summary>
        /// Gets or sets the world server's language.
        /// </summary>
        [DataMember(Name = "language")]
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets the IPC configuration.
        /// </summary>
        [DataMember(Name = "isc")]
        public ISCConfiguration ISC { get; set; }

        [DataMember(Name = "mailShippingCost")]
        public uint MailShippingCost { get; set; }

        /// <summary>
        /// Creates a new <see cref="WorldConfiguration"/> instance.
        /// </summary>
        public WorldConfiguration()
        {
            this.Language = DefaultLanguage;
            this.Systems = new Dictionary<string, bool>();
            this.ISC = new ISCConfiguration();
            this.MailShippingCost = DefaultMailShippingCost;
        }
    }
}

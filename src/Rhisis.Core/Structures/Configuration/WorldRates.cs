using System.Runtime.Serialization;

namespace Rhisis.Core.Structures.Configuration
{
    /// <summary>
    /// Represents the world server's rates configuration section.
    /// </summary>
    [DataContract]
    public class WorldRates
    {
        public const int DefaultGoldRate = 1;
        public const int DefaultExperienceRate = 1;
        public const int DefaultDropRate = 1;

        /// <summary>
        /// Gets or sets the gold drop rate.
        /// </summary>
        [DataMember(Name = "gold")]
        public int Gold { get; set; }

        /// <summary>
        /// Gets or sets the experience rate.
        /// </summary>
        [DataMember(Name = "experience")]
        public int Experience { get; set; }

        /// <summary>
        /// Gets or sets the drop rate.
        /// </summary>
        [DataMember(Name = "drop")]
        public int Drop { get; set; }

        /// <summary>
        /// Creates a new <see cref="WorldRates"/> instance.
        /// </summary>
        public WorldRates()
        {
            this.Gold = DefaultGoldRate;
            this.Experience = DefaultExperienceRate;
            this.Drop = DefaultDropRate;
        }
    }
}

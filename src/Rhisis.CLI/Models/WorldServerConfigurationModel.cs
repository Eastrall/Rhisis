using Newtonsoft.Json;
using Rhisis.Core.Structures.Configuration;
using Rhisis.Core.Structures.Configuration.World;

namespace Rhisis.CLI.Models
{
    class WorldServerConfigurationModel
    {
        [JsonProperty(PropertyName = ConfigurationConstants.WorldServer)]
        public WorldConfiguration WorldConfiguration { get; set; }
        [JsonProperty(PropertyName = ConfigurationConstants.CoreServer)]
        public CoreConfiguration CoreConfiguration { get; set; }
    }
}

using Newtonsoft.Json;

namespace Rhisis.Core.Structures.Configuration.Models
{
    public class ClusterServerConfigurationModel
    {
        [JsonProperty(PropertyName = ConfigurationConstants.ClusterServer)]
        public ClusterConfiguration ClusterServerConfiguration { get; set; }

        [JsonProperty(PropertyName = ConfigurationConstants.CoreServer)]
        public CoreConfiguration CoreConfiguration { get; set; }
    }
}

using Newtonsoft.Json;

namespace Rhisis.Core.Structures.Configuration.Models
{
    public class LoginServerConfigurationModel
    {
        [JsonProperty(PropertyName = ConfigurationConstants.LoginServer)]
        public LoginConfiguration LoginConfiguration { get; set; }
        [JsonProperty(PropertyName = ConfigurationConstants.CoreServer)]
        public CoreConfiguration CoreConfiguration { get; set; }
    }
}

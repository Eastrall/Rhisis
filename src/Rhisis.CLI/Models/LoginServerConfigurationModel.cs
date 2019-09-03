using Newtonsoft.Json;
using Rhisis.Core.Structures.Configuration;

namespace Rhisis.CLI.Models
{
    class LoginServerConfigurationModel
    {
        [JsonProperty(PropertyName = ConfigurationConstants.LoginServer)]
        public LoginConfiguration LoginConfiguration { get; set; }
        [JsonProperty(PropertyName = ConfigurationConstants.CoreServer)]
        public CoreConfiguration CoreConfiguration { get; set; }
    }
}

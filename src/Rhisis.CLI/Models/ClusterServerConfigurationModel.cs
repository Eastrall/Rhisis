using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Rhisis.Core.Structures.Configuration;

namespace Rhisis.CLI.Models
{
    class ClusterServerConfigurationModel
    {
        [JsonProperty(PropertyName = ConfigurationConstants.ClusterServer)]
        public ClusterConfiguration ClusterServerConfiguration { get; set; }

        [JsonProperty(PropertyName = ConfigurationConstants.CoreServer)]
        public CoreConfiguration CoreConfiguration { get; set; }
    }
}

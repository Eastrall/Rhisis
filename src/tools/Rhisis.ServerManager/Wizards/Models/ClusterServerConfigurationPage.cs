using Rhisis.Core.Structures.Configuration;

namespace Rhisis.ServerManager.Wizards.Models
{
    public class ClusterServerConfigurationPage : WizardConfigurationPageBase<ClusterConfiguration>
    {
        public ClusterServerConfigurationPage() : base("cluster.json")
        {
            Title = "Cluster Server";
            Description = "Configures the cluster server";
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public bool EnableLoginProtect { get; set; }

    }
}

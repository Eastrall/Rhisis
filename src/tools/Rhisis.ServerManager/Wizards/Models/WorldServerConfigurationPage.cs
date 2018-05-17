using Rhisis.Core.Structures.Configuration;
using Rhisis.ServerManager.Models;
using System.Collections.ObjectModel;

namespace Rhisis.ServerManager.Wizards.Models
{
    public class WorldServerConfigurationPage : WizardConfigurationPageBase<WorldConfiguration>
    {
        public WorldServerConfigurationPage() : base("world.json")
        {
            Title = "World Server";
            Description = "Configure the world server";
            WorldSystems = new ObservableCollection<WorldSystem>(new[]
            {
                new WorldSystem { Name = "System 1" },
                new WorldSystem { Name = "System 2" },
                new WorldSystem { Name = "System 3" },
                new WorldSystem { Name = "System 4" },
            });
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public ObservableCollection<WorldSystem> WorldSystems { get; }

    }
}

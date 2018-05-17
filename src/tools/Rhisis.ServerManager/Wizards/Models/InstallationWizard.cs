using Catel.IoC;
using Orc.Wizard;
using System.Windows;

namespace Rhisis.ServerManager.Wizards.Models
{
    public class InstallationWizard : WizardBase
    {
        public InstallationWizard(ITypeFactory typeFactory) : base(typeFactory)
        {
            Title = "Installation wizard";
            this.AddPage<DatabaseConfigurationPage>();
            this.AddPage<ISCConfigurationPage>();
            this.AddPage<LoginServerConfigurationPage>();
            this.AddPage<ClusterServerConfigurationPage>();
            this.AddPage<WorldServerConfigurationPage>();
            MinSize = new Size(650, 600);
        }

    }
}

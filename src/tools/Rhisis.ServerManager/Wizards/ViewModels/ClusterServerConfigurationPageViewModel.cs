using Catel.MVVM;
using Orc.Wizard;
using Rhisis.ServerManager.Wizards.Models;

namespace Rhisis.ServerManager.Wizards.ViewModels
{
    public class ClusterServerConfigurationPageViewModel : WizardPageViewModelBase<ClusterServerConfigurationPage>
    {
        public ClusterServerConfigurationPageViewModel(ClusterServerConfigurationPage wizardPage) : base(wizardPage)
        {
        }

        [ViewModelToModel]
        public string Host { get; set; }

        [ViewModelToModel]
        public int Port { get; set; }

        [ViewModelToModel]
        public int Id { get; set; }

        [ViewModelToModel]
        public string Name { get; set; }

        [ViewModelToModel]
        public bool EnableLoginProtect { get; set; }
    }
}

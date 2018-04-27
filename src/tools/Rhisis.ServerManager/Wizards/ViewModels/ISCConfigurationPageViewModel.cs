using Catel.MVVM;
using Orc.Wizard;
using Rhisis.ServerManager.Wizards.Models;

namespace Rhisis.ServerManager.Wizards.ViewModels
{
    // ReSharper disable once InconsistentNaming
    public class ISCConfigurationPageViewModel : WizardPageViewModelBase<ISCConfigurationPage>
    {
        public ISCConfigurationPageViewModel(ISCConfigurationPage wizardPage) : base(wizardPage)
        {
        }

        [ViewModelToModel]
        public string Host { get; set; }

        [ViewModelToModel]
        public int Port { get; set; }

        [ViewModelToModel]
        public string Password { get; set; }

    }
}

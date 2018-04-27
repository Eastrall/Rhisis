using Catel.MVVM;
using Orc.Wizard;
using Rhisis.ServerManager.Wizards.Models;

namespace Rhisis.ServerManager.Wizards.ViewModels
{
    public class LoginServerConfigurationPageViewModel : WizardPageViewModelBase<LoginServerConfigurationPage>
    {
        public LoginServerConfigurationPageViewModel(LoginServerConfigurationPage wizardPage) : base(wizardPage)
        {
        }

        [ViewModelToModel]
        public string Host { get; set; }

        [ViewModelToModel]
        public int Port { get; set; }

        [ViewModelToModel]
        public string BuildVersion { get; set; }

        [ViewModelToModel]
        public bool AccountVerification { get; set; }

        [ViewModelToModel]
        public bool PasswordEncryption { get; set; }

        [ViewModelToModel]
        public string EncryptionKey { get; set; }
    }
}

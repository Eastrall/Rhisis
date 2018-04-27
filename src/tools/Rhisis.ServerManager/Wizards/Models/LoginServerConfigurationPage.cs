using Rhisis.Core.Structures.Configuration;
using System.Threading.Tasks;

namespace Rhisis.ServerManager.Wizards.Models
{
    public class LoginServerConfigurationPage : WizardConfigurationPageBase<LoginConfiguration>
    {
        public LoginServerConfigurationPage() : base("login.json")
        {

            Title = "Login Server";
            Description = "Login server configuration";
        }

        public string BuildVersion { get; set; }

        public bool AccountVerification { get; set; }

        public bool PasswordEncryption { get; set; }

        public string EncryptionKey { get; set; }

        public override async Task Apply(LoginConfiguration configuration)
        {

        }
    }
}

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Rhisis.Core.Helpers;
using Rhisis.Core.Structures.Configuration;
using Rhisis.Database;
using Rhisis.Database.Entities;
using Rhisis.Installer.Enums;
using Rhisis.Installer.Models;
using Rhisis.Tools.Core.MVVM;

namespace Rhisis.Installer.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private const string DatabaseConfigurationPath = "config/database.json";
        private const string LoginConfigurationPath = "config/login.json";
        private const string ClusterConfigurationPath = "config/cluster.json";
        private const string WorldConfigurationPath = "config/world.json";

        private readonly DatabaseConfiguration _databaseConfiguration;
        private readonly LoginConfiguration _loginConfiguration;
        private readonly ClusterConfiguration _clusterConfiguration;
        private readonly WorldConfiguration _worldConfiguration;
        private readonly AccountModel _accountModel;

        private bool _installVisible;
        private string _currentLanguage;
        public string _currentStepInfo;
        private int _currentStep;

        public bool InstallVisible
        {
            get => this._installVisible;
            set => this.NotifyPropertyChanged(ref this._installVisible, value);
        }

        public string CurrentLanguage
        {
            get => this._currentLanguage;
            set => this.NotifyPropertyChanged(ref this._currentLanguage, value);
        }

        public string CurrentStepInfo
        {
            get => this._currentStepInfo;
            set => this.NotifyPropertyChanged(ref this._currentStepInfo, value);
        }

        public int CurrentStep
        {
            get => this._currentStep;
            set => this.NotifyPropertyChanged(ref this._currentStep, value);
        }

        public ICommand ConfigureCommand { get; }

        public ICommand StartInstallCommand { get; }

        public ICommand ChangeLanguageCommand { get; }

        public MainViewModel()
        {
            this.CheckConfigDirectory();
            this._databaseConfiguration = this.LoadConfiguration<DatabaseConfiguration>(DatabaseConfigurationPath);
            this._loginConfiguration = this.LoadConfiguration<LoginConfiguration>(LoginConfigurationPath);
            this._clusterConfiguration = this.LoadConfiguration<ClusterConfiguration>(ClusterConfigurationPath);
            this._worldConfiguration = this.LoadConfiguration<WorldConfiguration>(WorldConfigurationPath);
            this._accountModel = new AccountModel();
            this.ConfigureCommand = new Command(this.OnConfigure);
            this.StartInstallCommand = new Command(async () => await this.OnStartInstall());
            this.ChangeLanguageCommand = new Command(this.OnChangeLanguage);
            this.SetCurrentLanguage();
            this.CurrentStep = 0;
            this.InstallVisible = false;
        }

        private void OnConfigure(object parameter)
        {
            if (!(parameter is ConfigurationType configurationType))
                return;

            ViewModelBase selectedOptionViewModel = null;

            switch (configurationType)
            {
                case ConfigurationType.Database:
                    selectedOptionViewModel = new DatabaseConfigurationViewModel(this._databaseConfiguration, this._accountModel);
                    break;
                case ConfigurationType.Login:
                case ConfigurationType.Cluster:
                case ConfigurationType.World:
                    this.DialogService.ShowInformation(App.Instance.GetTranslation("NotImplementedTitle"), App.Instance.GetTranslation("NotImplementedMessage"));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(parameter), parameter, null);
            }

            selectedOptionViewModel?.ShowDialog();
        }

        private async Task OnStartInstall()
        {
            this.InstallVisible = true;
            await this.SetupDatabase();
            await this.CreateAccount();
            await this.SetupLogin();
            await this.SetupCluster();
            await this.SetupWorld();
            this.InstallVisible = false;
            this.DialogService.ShowInformation("Configuration success", "Rhisis has been configured!");
        }

        private void OnChangeLanguage(object parameter)
        {
            App.Instance.ChangeLanguage(parameter.ToString());
            this.SetCurrentLanguage();
        }

        private T LoadConfiguration<T>(string path) where T : class, new()
        {
            string fullPath = Path.Combine(Environment.CurrentDirectory, path);

            return File.Exists(fullPath) ? ConfigurationHelper.Load<T>(fullPath) : new T();
        }

        private void CheckConfigDirectory()
        {
            if (!Directory.Exists(Path.Combine(Environment.CurrentDirectory, "config")))
                Directory.CreateDirectory("config");
        }

        private void SetCurrentLanguage()
        {
            if (Thread.CurrentThread.CurrentUICulture.ToString() == "fr")
                this.CurrentLanguage = "/Rhisis.Installer;component/Resources/Images/french.png";
            else
                this.CurrentLanguage = "/Rhisis.Installer;component/Resources/Images/english.png";
        }

        private async Task SetupDatabase()
        {
            this.CurrentStepInfo = App.Instance.GetTranslation("ConfigureDatabase");

            ConfigurationHelper.Save(DatabaseConfigurationPath, this._databaseConfiguration);
            DatabaseService.Configure(this._databaseConfiguration);

            using (DatabaseContext context = DatabaseService.GetContext())
            {
                if (!context.DatabaseExists())
                {
                    bool result = this.DialogService.ShowQuestion(
                        App.Instance.GetTranslation("NoDatabaseTitle"), 
                        App.Instance.GetTranslation("NoDatabaseMessage", this._databaseConfiguration.Database));

                    if (result)
                    {
                        context.Migrate();
                    }
                }
            }

            DatabaseService.UnloadConfiguration();
            ConfigurationHelper.Save(DatabaseConfigurationPath, this._databaseConfiguration);
            await Task.Delay(500);
            
            this.CurrentStep++;
        }

        private async Task CreateAccount()
        {
            this.CurrentStepInfo = App.Instance.GetTranslation("ConfigureCreateAccount");

            if (this._accountModel.IsValid)
            {
                DatabaseService.Configure(this._databaseConfiguration);

                using (DatabaseContext context = DatabaseService.GetContext())
                {
                    User user = context.Users.Get(x => x.Username == this._accountModel.Username);

                    if (user != null)
                    {
                        this.DialogService.ShowError(
                            App.Instance.GetTranslation("CreateAccountErrorTitle"), 
                            App.Instance.GetTranslation("CreateAccountErrorUserAlreadyExists", this._accountModel.Username));
                    }
                    else
                    {
                        context.Users.Create(new User
                        {
                            Authority = (int)this._accountModel.Type,
                            Username = this._accountModel.Username,
                            Password = this._accountModel.Password
                        });

                        context.SaveChanges();
                    }
                }

                DatabaseService.UnloadConfiguration();
            }

            await Task.Delay(500);
            this.CurrentStep++;
        }

        private async Task SetupLogin()
        {
            this.CurrentStepInfo = App.Instance.GetTranslation("ConfigureLogin");

            this._loginConfiguration.BuildVersion = "20100412";
            this._loginConfiguration.AccountVerification = true;
            this._loginConfiguration.PasswordEncryption = true;
            this._loginConfiguration.EncryptionKey = "dldhsvmflvm";
            this._loginConfiguration.Host = "127.0.0.1";
            this._loginConfiguration.Port = 23000;
            this._loginConfiguration.ISC.Host = "127.0.0.1";
            this._loginConfiguration.ISC.Port = 15000;
            this._loginConfiguration.ISC.Password = "4fded1464736e77865df232cbcb4cd19";

            ConfigurationHelper.Save(LoginConfigurationPath, this._loginConfiguration);
            await Task.Delay(500);

            this.CurrentStep++;
        }

        private async Task SetupCluster()
        {
            this.CurrentStepInfo = App.Instance.GetTranslation("ConfigureCluster");

            this._clusterConfiguration.Id = 1;
            this._clusterConfiguration.Name = "Rhisis";
            this._clusterConfiguration.EnableLoginProtect = true;
            this._clusterConfiguration.Host = "127.0.0.1";
            this._clusterConfiguration.Port = 28000;
            this._clusterConfiguration.ISC.Host = "127.0.0.1";
            this._clusterConfiguration.ISC.Port = 15000;
            this._clusterConfiguration.ISC.Password = "4fded1464736e77865df232cbcb4cd19";

            this._clusterConfiguration.DefaultCharacter.Level = 1;
            this._clusterConfiguration.DefaultCharacter.Gold = 0;
            this._clusterConfiguration.DefaultCharacter.Strength = 15;
            this._clusterConfiguration.DefaultCharacter.Stamina = 15;
            this._clusterConfiguration.DefaultCharacter.Dexterity = 15;
            this._clusterConfiguration.DefaultCharacter.Intelligence = 15;
            this._clusterConfiguration.DefaultCharacter.MapId = 1;
            this._clusterConfiguration.DefaultCharacter.PosX = 6968;
            this._clusterConfiguration.DefaultCharacter.PosY = 100;
            this._clusterConfiguration.DefaultCharacter.PosZ = 3328;
            this._clusterConfiguration.DefaultCharacter.Man.StartWeapon = 21;
            this._clusterConfiguration.DefaultCharacter.Man.StartSuit = 502;
            this._clusterConfiguration.DefaultCharacter.Man.StartHand = 506;
            this._clusterConfiguration.DefaultCharacter.Man.StartShoes = 510;
            this._clusterConfiguration.DefaultCharacter.Man.StartHat = -1;
            this._clusterConfiguration.DefaultCharacter.Woman.StartWeapon = 21;
            this._clusterConfiguration.DefaultCharacter.Woman.StartSuit = 504;
            this._clusterConfiguration.DefaultCharacter.Woman.StartHand = 508;
            this._clusterConfiguration.DefaultCharacter.Woman.StartShoes = 512;
            this._clusterConfiguration.DefaultCharacter.Woman.StartHat = -1;

            ConfigurationHelper.Save(ClusterConfigurationPath, this._clusterConfiguration);
            await Task.Delay(500);

            this.CurrentStep++;
        }

        private async Task SetupWorld()
        {
            this.CurrentStepInfo = App.Instance.GetTranslation("ConfigureWorld");

            this._worldConfiguration.Id = 2;
            this._worldConfiguration.ClusterId = 1;
            this._worldConfiguration.Name = "Channel 1";
            this._worldConfiguration.Host = "127.0.0.1";
            this._worldConfiguration.Port = 5400;
            this._worldConfiguration.ISC.Host = "127.0.0.1";
            this._worldConfiguration.ISC.Port = 15000;
            this._worldConfiguration.ISC.Password = "4fded1464736e77865df232cbcb4cd19";
            this._worldConfiguration.Maps = new[] { "WI_WORLD_MADRIGAL" };

            ConfigurationHelper.Save(WorldConfigurationPath, this._worldConfiguration);
            await Task.Delay(500);

            this.CurrentStep++;
        }
    }
}

using Catel.Collections;
using Catel.MVVM;
using Orc.Wizard;
using Rhisis.ServerManager.Models;
using Rhisis.ServerManager.Wizards.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Rhisis.ServerManager.Wizards.ViewModels
{
    public class WorldServerConfigurationPageViewModel : WizardPageViewModelBase<WorldServerConfigurationPage>
    {
        public WorldServerConfigurationPageViewModel(WorldServerConfigurationPage wizardPage) : base(wizardPage)
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
        // ReSharper disable once UnusedAutoPropertyAccessor.Local /!\ Needed by Fody
        public ObservableCollection<WorldSystem> WorldSystems { get; private set; }

        protected override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            WorldSystems.ForEach(x => x.PropertyChanged += OnComponentPropertyChanged);
        }

        protected override async Task CloseAsync()
        {
            WorldSystems.ForEach(x => x.PropertyChanged -= OnComponentPropertyChanged);

            await base.CloseAsync();
        }

        private void OnComponentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Validate(true);
        }
    }
}

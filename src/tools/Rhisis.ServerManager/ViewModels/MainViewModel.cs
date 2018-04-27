using System.Threading.Tasks;
using Catel;
using Catel.IoC;
using Catel.MVVM;
using Orc.Wizard;
using Rhisis.ServerManager.Wizards.Models;

namespace Rhisis.ServerManager.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IWizardService _wizardService;
        private readonly ITypeFactory _typeFactory;

        public MainViewModel(IWizardService wizardService, ITypeFactory typeFactory)
        {
            Argument.IsNotNull(() => wizardService);
            Argument.IsNotNull(() => typeFactory);

            _wizardService = wizardService;
            _typeFactory = typeFactory;

            ShowWizard = new TaskCommand(OnShowWizardExecuteAsync);

            Title = "Orc.Wizard example";
        }

        #region Properties
        public bool ShowInTaskbar { get; set; }
        #endregion

        #region Commands
        public TaskCommand ShowWizard { get; private set; }

        private Task OnShowWizardExecuteAsync()
        {
            var wizard = _typeFactory.CreateInstance<InstallationWizard>();

            return _wizardService.ShowWizardAsync(wizard);
        }
        #endregion
    }
}

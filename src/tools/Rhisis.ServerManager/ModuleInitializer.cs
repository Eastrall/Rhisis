using Catel.IoC;
using Catel.MVVM;
using Rhisis.ServerManager.ViewModels;
using Rhisis.ServerManager.Wizards.ViewModels;
using DatabaseConfigurationPageView = Rhisis.ServerManager.Wizards.Views.DatabaseConfigurationPageView;
using MainView = Rhisis.ServerManager.Views.MainView;

namespace Rhisis.ServerManager
{
    /// <summary>
    /// Used by the ModuleInit. All code inside the Initialize method is ran as soon as the assembly is loaded.
    /// </summary>
    public static class ModuleInitializer
    {
        /// <summary>
        /// Initializes the module.
        /// </summary>
        public static void Initialize()
        {
            var serviceLocator = ServiceLocator.Default;
            var viewModelLocator = serviceLocator.ResolveType<IViewModelLocator>();
            viewModelLocator.Register(typeof(MainView), typeof(MainViewModel));
        }
    }
}

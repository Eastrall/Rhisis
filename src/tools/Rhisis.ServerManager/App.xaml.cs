

using AutoMapper;
using Rhisis.Core.Structures.Configuration;
using Rhisis.ServerManager.Wizards.Models;
using System.Windows;

namespace Rhisis.ServerManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            InitializeAutoMapper();
            base.OnStartup(e);
            //LogManager.AddDebugListener();
        }

        private void InitializeAutoMapper()
        {
            Mapper.Initialize(mapper =>
            {
                mapper.CreateMap<DatabaseConfigurationPage, DatabaseConfiguration>().ReverseMap();
                mapper.CreateMap<ClusterServerConfigurationPage, ClusterConfiguration>().ReverseMap();
                mapper.CreateMap<ISCConfigurationPage, ISCConfiguration>().ReverseMap();
                mapper.CreateMap<LoginServerConfigurationPage, LoginConfiguration>().ReverseMap();
                mapper.CreateMap<WorldServerConfigurationPage, WorldConfiguration>().ReverseMap();
            });
        }
    }
}

using Ether.Network.Packets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Rhisis.Core.DependencyInjection;
using Rhisis.Core.Extensions;
using Rhisis.Core.Resources;
using Rhisis.Core.Structures.Configuration;
using Rhisis.Database;
using Rhisis.Network.Packets;
using Rhisis.World.Game.Behaviors;
using Rhisis.World.Game.Maps;
using Sylver.HandlerInvoker;
using System.IO;
using System.Threading.Tasks;

namespace Rhisis.World
{
    public static class Program
    {
        private static async Task Main()
        {
            const string culture = "en-US";
            const string worldConfigurationPath = "config/world.json";
            const string databaseConfigurationPath = "config/database.json";

            var host = new HostBuilder()
                .ConfigureAppConfiguration((hostContext, configApp) =>
                {
                    configApp.SetBasePath(Directory.GetCurrentDirectory());
                    configApp.AddJsonFile(worldConfigurationPath, optional: false);
                    configApp.AddJsonFile(databaseConfigurationPath, optional: false);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddOptions();
                    services.AddMemoryCache();
                    services.Configure<WorldConfiguration>(hostContext.Configuration.GetSection("worldServer"));
                    services.Configure<ISCConfiguration>(hostContext.Configuration.GetSection("isc"));
                    services.RegisterDatabaseServices(hostContext.Configuration.Get<DatabaseConfiguration>());

                    services.AddHandlers();
                    services.AddGameResources();
                    services.AddInjectableServices();

                    // World server configuration
                    services.AddSingleton<IWorldServer, WorldServer>();
                    services.AddSingleton<IHostedService, WorldServerService>();
                })
                .ConfigureLogging(builder =>
                {
                    builder.AddFilter("Microsoft", LogLevel.Warning);
                    builder.SetMinimumLevel(LogLevel.Trace);
                    builder.AddNLog(new NLogProviderOptions
                    {
                        CaptureMessageTemplates = true,
                        CaptureMessageProperties = true
                    });
                })
                .UseConsoleLifetime()
                .SetConsoleCulture(culture)
                .Build();

            await host
                .AddHandlerParameterTransformer<INetPacketStream, IPacketDeserializer>((source, dest) =>
                {
                    dest?.Deserialize(source);
                    return dest;
                })
                .RunAsync();
        }
    }
}
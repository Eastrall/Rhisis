using Ether.Network.Packets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Rhisis.Cluster.CoreClient;
using Rhisis.Cluster.CoreClient.Packets;
using Rhisis.Cluster.Packets;
using Rhisis.Core.Extensions;
using Rhisis.Core.Resources;
using Rhisis.Core.Structures.Configuration;
using Rhisis.Database;
using Rhisis.Network.Packets;
using Sylver.HandlerInvoker;
using System.IO;
using System.Threading.Tasks;

namespace Rhisis.Cluster
{
    public static class Program
    {
        private static async Task Main()
        {
            const string culture = "en-US";
            const string clusterConfigurationPath = "config/cluster.json";
            const string databaseConfigurationPath = "config/database.json";

            var host = new HostBuilder()
                .ConfigureAppConfiguration((hostContext, configApp) =>
                {
                    configApp.SetBasePath(Directory.GetCurrentDirectory());
                    configApp.AddJsonFile(clusterConfigurationPath, optional: false);
                    configApp.AddJsonFile(databaseConfigurationPath, optional: false);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddOptions();
                    services.AddMemoryCache();
                    services.Configure<ClusterConfiguration>(hostContext.Configuration.GetSection("clusterServer"));
                    services.Configure<ISCConfiguration>(hostContext.Configuration.GetSection("isc"));
                    services.RegisterDatabaseServices(hostContext.Configuration.Get<DatabaseConfiguration>());

                    services.AddHandlers();
                    services.AddGameResources();

                    // Core client configuration
                    services.AddSingleton<IClusterCoreClient, ClusterCoreClient>();
                    services.AddSingleton<ICorePacketFactory, CorePacketFactory>();
                    services.AddSingleton<IHostedService, ClusterCoreClientService>();

                    // Cluster server configuration
                    services.AddSingleton<IClusterServer, ClusterServer>();
                    services.AddSingleton<IClusterPacketFactory, ClusterPacketFactory>();
                    services.AddSingleton<IHostedService, ClusterServerService>();
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
                .SetConsoleCulture(culture)
                .UseConsoleLifetime()
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
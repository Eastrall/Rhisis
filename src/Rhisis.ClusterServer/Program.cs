﻿using LiteNetwork;
using LiteNetwork.Client.Hosting;
using LiteNetwork.Hosting;
using LiteNetwork.Protocol.Abstractions;
using LiteNetwork.Server.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Rhisis.ClusterServer.Abstractions;
using Rhisis.ClusterServer.Core;
using Rhisis.ClusterServer.Packets;
using Rhisis.Core.Extensions;
using Rhisis.Core.Structures.Configuration;
using Rhisis.Game.Abstractions.Protocol;
using Rhisis.Game.Abstractions.Resources;
using Rhisis.Game.Resources;
using Rhisis.Infrastructure.Persistance;
using Rhisis.Network;
using Sylver.HandlerInvoker;
using System;
using System.Threading.Tasks;

namespace Rhisis.ClusterServer
{
    public static class Program
    {
        private static async Task Main()
        {
            const string culture = "en-US";

            var host = new HostBuilder()
                .ConfigureAppConfiguration((hostContext, configApp) =>
                {
                    configApp.SetBasePath(EnvironmentExtension.GetCurrentEnvironementDirectory());
                    configApp.AddJsonFile(ConfigurationConstants.ClusterServerPath, optional: false);
                    configApp.AddJsonFile(ConfigurationConstants.DatabasePath, optional: false);
                    configApp.AddEnvironmentVariables();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddOptions();
                    services.AddMemoryCache();

                    services.Configure<ClusterConfiguration>(hostContext.Configuration.GetSection(ConfigurationConstants.ClusterServer));
                    services.Configure<CoreConfiguration>(hostContext.Configuration.GetSection(ConfigurationConstants.CoreServer));

                    services.AddPersistance(hostContext.Configuration);
                    services.AddHandlers();
                    services.AddSingleton<IGameResources, GameResources>();
                    services.AddSingleton<IClusterPacketFactory, ClusterPacketFactory>();
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
                .ConfigureLiteNetwork((context, builder) =>
                {
                    builder.AddLiteServer<IClusterServer, ClusterServer, ClusterClient>(options =>
                    {
                        var serverOptions = context.Configuration.GetSection(ConfigurationConstants.ClusterServer).Get<ClusterConfiguration>();

                        if (serverOptions is null)
                        {
                            throw new InvalidProgramException($"Failed to load cluster server settings.");
                        }

                        options.Host = serverOptions.Host;
                        options.Port = serverOptions.Port;
                        options.PacketProcessor = new FlyffPacketProcessor();
                        options.ReceiveStrategy = ReceiveStrategyType.Queued;
                    });
                    builder.AddLiteClient<ClusterCoreClient>(options =>
                    {
                        var serverOptions = context.Configuration.GetSection(ConfigurationConstants.CoreServer).Get<CoreConfiguration>();

                        if (serverOptions is null)
                        {
                            throw new InvalidProgramException($"Failed to load cluster core server settings.");
                        }

                        options.Host = serverOptions.Host;
                        options.Port = serverOptions.Port;
                        options.ReceiveStrategy = ReceiveStrategyType.Queued;
                    });
                })
                .SetConsoleCulture(culture)
                .UseConsoleLifetime()
                .Build();

            await host
                .AddHandlerParameterTransformer<ILitePacketStream, IPacketDeserializer>((source, dest) =>
                {
                    dest?.Deserialize(source);
                    return dest;
                })
                .RunAsync();
        }
    }
}
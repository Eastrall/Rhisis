using McMaster.Extensions.CommandLineUtils;
using Rhisis.CLI.Services;
using Rhisis.Core.Helpers;
using Rhisis.Core.Structures.Configuration;
using Rhisis.Core.Structures.Configuration.World;
using System;
using Rhisis.CLI.Models;

namespace Rhisis.CLI.Commands.Configure
{
    [Command("world", Description = "Configures the World Server")]
    public sealed class WorldServerConfigurationCommand
    {
        private readonly ConsoleHelper _consoleHelper;

        /// <summary>
        /// Gets or sets the cluster configuration file to use.
        /// </summary>
        [Option(CommandOptionType.SingleValue, ShortName = "c", LongName = "configuration", Description = "Specify the world server configuration file path.")]
        public string WorldConfigurationFile { get; set; }

        /// <summary>
        /// Gets the real configuration file.
        /// </summary>
        private string ConfigurationFile => !string.IsNullOrEmpty(this.WorldConfigurationFile) ? this.WorldConfigurationFile : ConfigurationConstants.WorldServerPath;


        /// <summary>
        /// Creates a new <see cref="WorldServerConfigurationCommand"/> instance.
        /// </summary>
        /// <param name="consoleHelper">Console helpers.</param>
        public WorldServerConfigurationCommand(ConsoleHelper consoleHelper)
        {
            this._consoleHelper = consoleHelper;
        }

        /// <summary>
        /// Executes the "configure world" command.
        /// </summary>
        public void OnExecute()
        {
            var worldServerConfiguration = ConfigurationHelper.Load<WorldConfiguration>(this.ConfigurationFile, ConfigurationConstants.WorldServer);
            var coreServerConfiguration = ConfigurationHelper.Load<CoreConfiguration>(this.ConfigurationFile, ConfigurationConstants.CoreServer);
            var worldConfiguration = new ObjectConfigurationFiller<WorldConfiguration>(worldServerConfiguration);
            var coreConfiguration = new ObjectConfigurationFiller<CoreConfiguration>(coreServerConfiguration);

            Console.WriteLine("----- World Server -----");
            worldConfiguration.Fill();
            Console.WriteLine("----- Core Server -----");
            coreConfiguration.Fill();

            Console.WriteLine("##### Configuration review #####");
            worldConfiguration.Show("World Server configuration");
            coreConfiguration.Show("Core server configuration");

            bool response = this._consoleHelper.AskConfirmation("Save this configuration?");

            if (!response) return;
            ConfigurationHelper.Save(ConfigurationConstants.WorldServerPath, new WorldServerConfigurationModel()
            {
                CoreConfiguration = coreConfiguration.Value,
                WorldConfiguration = worldConfiguration.Value
            });
            Console.WriteLine($"World Server configuration saved in {ConfigurationConstants.WorldServerPath}!");
        }
    }
}

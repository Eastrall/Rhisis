using McMaster.Extensions.CommandLineUtils;
using Rhisis.CLI.Services;
using Rhisis.Core.Helpers;
using Rhisis.Core.Structures.Configuration;
using System;
using System.Collections.Generic;
using Rhisis.CLI.Core;
using Rhisis.Core.Structures.Configuration.Models;


namespace Rhisis.CLI.Commands.Configure
{
    [Command("login", Description = "Configures the Login Server")]
    public class LoginServerConfigurationCommand
    {
        private readonly ConsoleHelper _consoleHelper;

        /// <summary>
        /// Gets or sets the login configuration file to use.
        /// </summary>
        [Option(CommandOptionType.SingleValue, ShortName = "c", LongName = "configuration", Description = "Specify the login server configuration file path.")]
        public string LoginConfigurationFile { get; set; }

        /// <summary>
        /// Gets the real configuration file.
        /// </summary>
        private string ConfigurationFile => !string.IsNullOrEmpty(this.LoginConfigurationFile) ? this.LoginConfigurationFile : ConfigurationConstants.LoginServerPath;

        /// <summary>
        /// Creates a new <see cref="LoginServerConfigurationCommand"/> instance.
        /// </summary>
        /// <param name="consoleHelper"></param>
        public LoginServerConfigurationCommand(ConsoleHelper consoleHelper)
        {
            this._consoleHelper = consoleHelper;
        }

        /// <summary>
        /// Executes the "configure login" command.
        /// </summary>
        public void OnExecute()
        {
            var loginServerConfiguration = ConfigurationHelper.Load<LoginConfiguration>(this.ConfigurationFile, ConfigurationConstants.LoginServer);
            var coreServerConfiguration = ConfigurationHelper.Load<CoreConfiguration>(this.ConfigurationFile, ConfigurationConstants.CoreServer);
            var loginConfiguration = new ObjectConfigurationFiller<LoginConfiguration>(loginServerConfiguration);
            var coreConfiguration = new ObjectConfigurationFiller<CoreConfiguration>(coreServerConfiguration);

            Console.WriteLine("----- Login Server -----");
            loginConfiguration.Fill();
            Console.WriteLine("----- Core Server -----");
            coreConfiguration.Fill();

            Console.WriteLine("##### Configuration review #####");
            loginConfiguration.Show("Login Server configuration");
            coreConfiguration.Show("Core server configuration");

            bool response = this._consoleHelper.AskConfirmation("Save this configuration?");

            if (!response) 
                return;
            var configuration = new Dictionary<string, object>
            {
                { ConfigurationConstants.LoginServer, loginConfiguration.Value },
                { ConfigurationConstants.CoreServer, coreConfiguration.Value }
            };

            ConfigurationHelper.Save(this.ConfigurationFile, new LoginServerConfigurationModel
            {
                CoreConfiguration = coreConfiguration.Value,
                LoginConfiguration = loginConfiguration.Value
            });
            Console.WriteLine($"Login Server configuration saved in {this.ConfigurationFile}!");
        }
    }
}

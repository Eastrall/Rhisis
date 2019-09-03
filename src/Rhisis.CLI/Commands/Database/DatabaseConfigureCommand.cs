using System;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.EntityFrameworkCore.DataEncryption.Providers;
using Rhisis.CLI.Services;
using Rhisis.Core.Helpers;
using Rhisis.Database;

namespace Rhisis.CLI.Commands.Database
{
    [Command("configure", Description = "Configures the database access")]
    public class DatabaseConfigureCommand
    {       
        [Option(CommandOptionType.SingleValue, ShortName = "c", LongName = "configuration", Description = "Specify the database configuration file path.")]
        public string DatabaseConfigurationFile { get; set; }
        
        private readonly ConsoleHelper _consoleHelper;

        public DatabaseConfigureCommand(ConsoleHelper consoleHelper)
        {
            _consoleHelper = consoleHelper;
        }

        public void OnExecute()
        {
            if (string.IsNullOrEmpty(DatabaseConfigurationFile))
                DatabaseConfigurationFile = Application.DefaultDatabaseConfigurationFile;
            
            var dbConfiguration = new DatabaseConfiguration()
            {
                EncryptionKey = Convert.ToBase64String(AesProvider.GenerateKey(AesKeySize.AES256Bits).Key)
            };

            Console.WriteLine("### Database configuration ###");

            Console.Write("Host (localhost): ");
            dbConfiguration.Host = _consoleHelper.ReadStringOrDefault("localhost");

            Console.Write("Port (3306): ");
            dbConfiguration.Port = _consoleHelper.ReadIntegerOrDefault(3306);

            Console.Write("Username (root): ");
            dbConfiguration.Username = _consoleHelper.ReadStringOrDefault("root");

            Console.Write("Password: ");
            dbConfiguration.Password = _consoleHelper.ReadPassword();

            Console.Write("Database name (rhisis): ");
            dbConfiguration.Database = _consoleHelper.ReadStringOrDefault("rhisis");

            Console.WriteLine("--------------------------------");
            Console.WriteLine("Configuration:");
            Console.WriteLine($"Host: {dbConfiguration.Host}");
            Console.WriteLine($"Username: {dbConfiguration.Username}");
            Console.WriteLine($"Database name: {dbConfiguration.Database}");
            Console.WriteLine($"Port: {dbConfiguration.Port}");
            Console.WriteLine("--------------------------------");
            
            bool response = _consoleHelper.AskConfirmation("Save this configuration?");

            if (!response) return;
            ConfigurationHelper.Save(DatabaseConfigurationFile, dbConfiguration);
            Console.WriteLine($"Database configuration saved in '{DatabaseConfigurationFile}'.");
        }
    }
}

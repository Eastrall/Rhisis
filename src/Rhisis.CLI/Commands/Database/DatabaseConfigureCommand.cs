using McMaster.Extensions.CommandLineUtils;
using Rhisis.CLI.Helpers;
using Rhisis.Core.Helpers;
using Rhisis.Database;
using System;

namespace Rhisis.CLI.Commands.Database
{
    [Command("configure", Description = "Configures the database access")]
    public class DatabaseConfigureCommand
    {
        [Option(CommandOptionType.SingleValue, ShortName = "c", LongName = "configuration", Description = "Specify the database configuration file path.")]
        public string DatabaseConfigurationFile { get; set; }

        public void OnExecute(CommandLineApplication app, IConsole console)
        {
            if (string.IsNullOrEmpty(DatabaseConfigurationFile))
                DatabaseConfigurationFile = "config/database.json";

            var dbConfiguration = new DatabaseConfiguration();

            Console.WriteLine("Select one of the available providers:");
            ConsoleHelper.DisplayEnum<DatabaseProvider>();
            Console.Write("Database provider: ");
            dbConfiguration.Provider = ConsoleHelper.ReadEnum<DatabaseProvider>();

            if (dbConfiguration.Provider == DatabaseProvider.MySql)
            {
                Console.Write("Port (3306): ");
                dbConfiguration.Port = ConsoleHelper.ReadIntegerOrDefault(3306);
            }

            Console.Write("Host (localhost): ");
            dbConfiguration.Host = ConsoleHelper.ReadStringOrDefault("localhost");

            Console.Write("Username (root): ");
            dbConfiguration.Username = ConsoleHelper.ReadStringOrDefault("root");

            Console.Write("Password: ");
            dbConfiguration.Password = ConsoleHelper.ReadPassword();

            Console.Write("Database name (rhisis): ");
            dbConfiguration.Database = ConsoleHelper.ReadStringOrDefault("rhisis");

            Console.WriteLine("--------------------------------");
            Console.WriteLine("Configuration:");
            Console.WriteLine($"Database Provider: {dbConfiguration.Provider.ToString()}");
            Console.WriteLine($"Host: {dbConfiguration.Host}");
            Console.WriteLine($"Username: {dbConfiguration.Username}");
            Console.WriteLine($"Database name: {dbConfiguration.Database}");

            if (dbConfiguration.Provider == DatabaseProvider.MySql)
                Console.WriteLine($"Port: {dbConfiguration.Port}");

            Console.WriteLine("--------------------------------");
            
            bool response = ConsoleHelper.AskConfirmation("Save this configuration?");

            if (response)
            {
                ConfigurationHelper.Save(DatabaseConfigurationFile, dbConfiguration);
                Console.WriteLine($"Database configuration saved in '{DatabaseConfigurationFile}'.");
            }
        }
    }
}

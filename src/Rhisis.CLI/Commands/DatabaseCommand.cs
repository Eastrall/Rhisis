using McMaster.Extensions.CommandLineUtils;
using Rhisis.CLI.Helpers;
using Rhisis.Core.Helpers;
using Rhisis.Database;
using System;

namespace Rhisis.CLI.Commands
{
    [Command(Name = "database", Description = "Manages the database")]
    [Subcommand("configure", typeof(DatabaseConfigureCommand))]
    [Subcommand("update", typeof(DatabaseUpdateCommand))]
    [Subcommand("initialize", typeof(DatabaseInitializationCommand))]
    public class DatabaseCommand
    {
        public void OnExecute(CommandLineApplication app, IConsole console)
        {
            app.ShowHelp();
        }
    }

    [Command(Name = "configure", Description = "Configures the database access")]
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

            Console.WriteLine("Save this configuration ? (y/n)");
            string response = Console.ReadLine();

            if (response.Equals("y", StringComparison.OrdinalIgnoreCase))
            {
                ConfigurationHelper.Save(DatabaseConfigurationFile, dbConfiguration);
                Console.WriteLine($"Database configuration saved in '{DatabaseConfigurationFile}'.");
            }
        }
    }

    [Command(Name = "update", Description = "Updates the database structure")]
    public class DatabaseUpdateCommand
    {
        [Option(CommandOptionType.SingleValue, ShortName = "c", LongName = "configuration", Description = "Specify the database configuration file path.")]
        public string DatabaseConfigurationFile { get; set; }

        public void OnExecute(CommandLineApplication app, IConsole console)
        {
            if (string.IsNullOrEmpty(DatabaseConfigurationFile))
                DatabaseConfigurationFile = "config/database.json";

            try
            {
                Console.WriteLine("Starting database structure update...");
                DatabaseFactory.Instance.Initialize(DatabaseConfigurationFile);
                
                using (var rhisisDbContext = DatabaseFactory.Instance.CreateDbContext())
                {
                    if (rhisisDbContext.DatabaseExists())
                    {
                        rhisisDbContext.Migrate();
                        Console.WriteLine("Database updated.");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }

    [Command(Name = "initialize", Description = "Configures and initialize the database.")]
    public class DatabaseInitializationCommand
    {
        private readonly DatabaseConfigureCommand _configurationCommand;
        private readonly DatabaseUpdateCommand _updateCommand;
        
        [Option(CommandOptionType.SingleValue, ShortName = "c", LongName = "configuration", Description = "Specify the database configuration file path.")]
        public string DatabaseConfigurationFile { get; set; }

        public DatabaseInitializationCommand()
        {
            this._configurationCommand = new DatabaseConfigureCommand
            {
                DatabaseConfigurationFile = this.DatabaseConfigurationFile
            };
            this._updateCommand = new DatabaseUpdateCommand
            {
                DatabaseConfigurationFile = this.DatabaseConfigurationFile
            };
        }

        public void OnExecute(CommandLineApplication app, IConsole console)
        {
            this._configurationCommand.OnExecute(app, console);
            this._updateCommand.OnExecute(app, console);
        }
    }
}

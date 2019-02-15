using McMaster.Extensions.CommandLineUtils;
using Rhisis.Database;
using System;

namespace Rhisis.CLI.Commands.Database
{
    [Command("update", Description = "Updates the database structure")]
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
}

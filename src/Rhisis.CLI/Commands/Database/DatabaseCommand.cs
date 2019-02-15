using McMaster.Extensions.CommandLineUtils;

namespace Rhisis.CLI.Commands.Database
{
    [Command("database", Description = "Manages the database")]
    [Subcommand(typeof(DatabaseConfigureCommand))]
    [Subcommand(typeof(DatabaseUpdateCommand))]
    [Subcommand(typeof(DatabaseInitializationCommand))]
    public class DatabaseCommand
    {
        public void OnExecute(CommandLineApplication app, IConsole console)
        {
            app.ShowHelp();
        }
    }
}

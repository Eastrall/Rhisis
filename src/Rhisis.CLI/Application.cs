using McMaster.Extensions.CommandLineUtils;
using Rhisis.CLI.Commands.Database;

namespace Rhisis.CLI
{
    [Command(ThrowOnUnexpectedArgument = false, Description = Program.Description)]
    [Subcommand(typeof(DatabaseCommand))]
    public class Application
    {
        public void OnExecute(CommandLineApplication app, IConsole console)
        {
            app.ShowHelp();
        }
    }
}

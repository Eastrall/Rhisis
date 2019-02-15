using McMaster.Extensions.CommandLineUtils;

namespace Rhisis.CLI.Commands.Database
{
    [Command("initialize", Description = "Configures and initialize the database.")]
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

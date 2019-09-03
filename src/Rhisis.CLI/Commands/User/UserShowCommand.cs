using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;
using Rhisis.Core.Helpers;
using Rhisis.Database;
using Rhisis.Database.Entities;

namespace Rhisis.CLI.Commands.User
{
    [Command("show", Description = "Show an user.")]
    public sealed class UserShowCommand
    {
        private readonly IDatabase _database;

        [Required]
        [Argument(0)]
        public string Username { get; set; }

        [Option(CommandOptionType.SingleValue, ShortName = "c", LongName = "configuration", Description = "Specify the database configuration file path.")]
        public string DatabaseConfigurationFile { get; set; }
        
        public UserShowCommand(DatabaseFactory databaseFactory)
        {
            if (string.IsNullOrEmpty(DatabaseConfigurationFile))
                DatabaseConfigurationFile = Application.DefaultDatabaseConfigurationFile;
            
            var dbConfig = ConfigurationHelper.Load<DatabaseConfiguration>(DatabaseConfigurationFile);
            _database = databaseFactory.GetDatabase(dbConfig);
        }

        public void OnExecute()
        {
            DbUser user = this._database.Users.Get(x => x.Username.Equals(this.Username, StringComparison.OrdinalIgnoreCase));

            if (user == null)
            {
                Console.WriteLine($"Cannot find user with username: '{this.Username}'.");
            }
            else
            {
                Console.WriteLine("#########################");
                Console.WriteLine("#   User information's   #");
                Console.WriteLine("#########################");
                Console.WriteLine($"Username: {user.Username}");
                Console.WriteLine($"Email: {user.Email}");
                Console.WriteLine($"Authority: {user.Authority.ToString()}");
                Console.WriteLine($"Deleted: {user.IsDeleted}");
                Console.WriteLine($"Last connection: {user.LastConnectionTime.ToString("yyyy/MM/dd HH:mm:ss")}");
                Console.WriteLine($"Play time: {TimeSpan.FromSeconds(user.PlayTime).ToString(@"hh\:mm\:ss")}");
                Console.WriteLine($"Number of characters: {user.Characters.Count}");

                if (user.Characters.Any())
                {
                    for (int i = 0; i < user.Characters.Count; i++)
                    {
                        DbCharacter character = user.Characters.ElementAt(i);

                        Console.WriteLine("-------------------------");
                        Console.WriteLine($"Character name: {character.Name} (id: {character.Id})");
                        Console.WriteLine($"Deleted: {character.IsDeleted}");
                    }
                }
            }
        }
    }
}

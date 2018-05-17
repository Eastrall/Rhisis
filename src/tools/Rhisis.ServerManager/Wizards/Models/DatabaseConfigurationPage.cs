﻿using Rhisis.Core.Structures.Configuration;

namespace Rhisis.ServerManager.Wizards.Models
{
    public class DatabaseConfigurationPage : WizardConfigurationPageBase<DatabaseConfiguration>
    {
        public DatabaseConfigurationPage() : base("database.json")
        {
            Title = "Database";
            Description = "Configures the database";
            IsOptional = false;
        }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Database { get; set; }

        public DatabaseProvider Provider { get; set; }

    }
}

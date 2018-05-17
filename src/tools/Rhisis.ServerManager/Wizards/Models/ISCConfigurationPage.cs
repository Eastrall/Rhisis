﻿using Rhisis.Core.Structures.Configuration;

namespace Rhisis.ServerManager.Wizards.Models
{
    public class ISCConfigurationPage : WizardConfigurationPageBase<ISCConfiguration>
    {
        public ISCConfigurationPage() : base("isc.json")
        {
            Title = "ISC";
            Description = "Inter-server connection configuration";
        }

        public string Password { get; set; }

    }
}

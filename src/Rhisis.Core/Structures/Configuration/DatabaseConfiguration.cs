using System.ComponentModel;
using System.Runtime.Serialization;

namespace Rhisis.Core.Structures.Configuration
{
    [DataContract]
    public class DatabaseConfiguration : BaseConfiguration
    {
        [DataMember(Name = "username")]
        [DefaultValue("root")]
        public string Username { get; set; }

        [DataMember(Name = "password")]
        [DefaultValue("")]
        public string Password { get; set; }

        [DataMember(Name = "database")]
        [DefaultValue("rhisis")]
        public string Database { get; set; }

        [DataMember(Name = "provider")]
        [DefaultValue(DatabaseProvider.MySQL)]
        public DatabaseProvider Provider { get; set; }

        [IgnoreDataMember]
        public bool IsValid { get; set; }
    }
}

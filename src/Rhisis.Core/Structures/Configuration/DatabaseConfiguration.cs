using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Rhisis.Core.Structures.Configuration
{
    [DataContract]
    public sealed class DatabaseConfiguration
    {
        /// <summary>
        /// Gets or sets the database host.
        /// </summary>
        [DataMember(Name = "host")]
        [DefaultValue("127.0.0.1")]
        [Display(Name = "Database server host address", Order = 0)]
        public string Host { get; set; }

        /// <summary>
        /// Gets or sets the database port.
        /// </summary>
        [DataMember(Name = "port")]
        [DefaultValue(3306)]
        [Display(Name = "Database server listening port", Order = 1)]
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets the database connection username.
        /// </summary>
        [DataMember(Name = "username")]
        [DefaultValue("root")]
        [Display(Name = "Database username", Order = 2)]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the database connection password
        /// </summary>
        [DataMember(Name = "password")]
        [Display(Name = "Database user password", Order = 3)]
        [PasswordPropertyText]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the database name.
        /// </summary>
        [DataMember(Name = "database")]
        [DefaultValue("rhisis")]
        [Display(Name = "Database name", Order = 4)]
        public string Database { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates if the database should use encryption or not.
        /// </summary>
        [DataMember(Name = "useEncryption")]
        [DefaultValue(false)]
        [Display(Name = "Use encryption", Order = 5)]
        public bool UseEncryption { get; set; }

        /// <summary>
        /// Gets or sets the database encryption key.
        /// </summary>
        /// <remarks>
        /// This key will be used to encrypt every string fields of the database tables.
        /// </remarks>
        [DataMember(Name = "encryptionKey")]
        [NotMapped]
        public string EncryptionKey { get; set; }

        /// <summary>
        /// (optional) Get or sets if the public key may be retrieved by the mysql connector.
        /// </summary>
        /// <remarks>
        /// Check the mysql connector documentation for details: https://mysqlconnector.net/connection-options/
        /// </remarks>
        [DataMember(Name = "allowPublicKeyRetrieval")]
        [DefaultValue(false)]
        [Display(Name = "Allow public key retrieval", Order = 6)]
        public bool AllowPublicKeyRetrieval { get; set; }

        /// <summary>
        /// (optional) Get or sets the RSA public key file to be used by the mysql connector.
        /// </summary>
        /// <remarks>
        /// Check the mysql connector documentation for details: https://mysqlconnector.net/connection-options/
        /// </remarks>
        [DataMember(Name = "serverRSAPublicKeyFile")]
        [DefaultValue(null)]
        [Display(Name = "Server RSA public key file", Order = 7)]
        public string ServerRSAPublicKeyFile { get; set; }

        /// <summary>
        /// Gets or sets the valid state of the configuration.
        /// </summary>
        [IgnoreDataMember]
        [NotMapped]
        public bool IsValid { get; set; }

        /// <summary>
        /// Gets or sets the MySQL server version number.
        /// </summary>
        [DataMember(Name = "serverVersion")]
        [Display(Name = "MySQL Server version", Order = 8)]
        public Version ServerVersion { get; set; } = new Version(5, 7, 32);

        /// <inheritdoc />
        public override string ToString() 
            => $"Host: {Host}, Port: {Port}, Username: {Username}, Password: {Password}, Database: {Database}";
    }
}

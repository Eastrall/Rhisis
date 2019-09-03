namespace Rhisis.Core.Structures.Configuration.Models
{
    public class DatabaseConfigurationModel
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Database { get; set; }
        public string EncryptionKey { get; set; }
        public bool IsValid { get; set; }
    }
}

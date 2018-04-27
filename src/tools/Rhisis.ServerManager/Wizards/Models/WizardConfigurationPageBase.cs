using AutoMapper;
using Newtonsoft.Json;
using Orc.Wizard;
using Rhisis.Core.Structures.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace Rhisis.ServerManager.Wizards.Models
{
    public abstract class WizardConfigurationPageBase<TConfiguration> : WizardPageBase where TConfiguration : BaseConfiguration
    {
        private readonly string _filePath;

        protected WizardConfigurationPageBase(string filePath)
        {
            _filePath = filePath;
            Load();
        }

        public string Host { get; set; }

        public int Port { get; set; }

        public TConfiguration ToConfiguration()
        {
            return Mapper.Map<TConfiguration>(this);
        }

        protected virtual void Load()
        {
            if (!File.Exists(_filePath))
            {
                return;
            }
            var serializedConfiguration = File.ReadAllText(_filePath);
            var configuration = JsonConvert.DeserializeObject<TConfiguration>(serializedConfiguration);
            Mapper.Map(configuration, this);
        }

        public override Task SaveAsync()
        {
            var configuration = ToConfiguration();
            var serializedConfiguration = JsonConvert.SerializeObject(configuration, Formatting.Indented);
            File.WriteAllText(_filePath, serializedConfiguration);
            return Apply(configuration);
        }

        public virtual Task Apply(TConfiguration configuration)
        {
            return Task.CompletedTask;
        }
    }
}
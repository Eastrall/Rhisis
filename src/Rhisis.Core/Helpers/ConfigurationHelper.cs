using Newtonsoft.Json;
using Rhisis.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Rhisis.Core.Helpers
{
    public static class ConfigurationHelper
    {
        public static T Load<T>(string path) where T : class, new()
        {
            return Load<T>(path, false);
        }

        public static T Load<T>(string path, bool createIfNotExists) where T : class, new()
        {
            if (!File.Exists(path))
            {
                if (createIfNotExists)
                    Save(path, new T());
                else
                    throw new RhisisConfigurationException(path);
            }

            string fileContent = File.ReadAllText(path);

            return JsonConvert.DeserializeObject<T>(fileContent, new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            });
        }

        public static void Save<T>(string path, T value) where T : class, new()
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(Path.GetDirectoryName(path));

            string valueSerialized = JsonConvert.SerializeObject(value, Formatting.Indented);

            File.WriteAllText(path, valueSerialized);
        }
    }
}

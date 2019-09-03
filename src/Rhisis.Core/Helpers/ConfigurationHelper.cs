using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Rhisis.Core.Exceptions;
using System;
using System.IO;

namespace Rhisis.Core.Helpers
{
    public static class ConfigurationHelper
    {
        /// <summary>
        /// Loads and deserializes a json file and converts it into a <typeparamref name="T"/> object.
        /// </summary>
        /// <typeparam name="T">Target object type.</typeparam>
        /// <param name="path">File path to deserialize.</param>
        /// <returns></returns>
        public static T Load<T>(string path) where T : class, new()
        {
            if (!File.Exists(path))
            {
                throw new RhisisConfigurationException(path);
            }

            string fileContent = File.ReadAllText(path);

            return JsonConvert.DeserializeObject<T>(fileContent, new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
        }

        /// <summary>
        /// Loads and deserializes a JSON file and converts the <paramref name="jsonKey"/> into a <typeparamref name="T"/> object.
        /// </summary>
        /// <typeparam name="T">Target object type.</typeparam>
        /// <param name="path">File path to deserialize.</param>
        /// <param name="jsonKey">JSON key.</param>
        /// <returns></returns>
        public static T Load<T>(string path, string jsonKey)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path), $"Cannot open an empty path.");
            }

            if (!File.Exists(path))
            {
                return default;
            }

            string fileContent = File.ReadAllText(path);
            var jsonObject = JObject.Parse(fileContent);

            if (jsonObject == null)
            {
                return default;
            }

            return jsonObject.TryGetValue(jsonKey, out JToken value) ? value.ToObject<T>() : default;
        }

        /// <summary>
        /// Saves an object as a JSON file.
        /// </summary>
        /// <typeparam name="T">Target object type.</typeparam>
        /// <param name="path">File path.</param>
        /// <param name="value">Object to save.</param>
        public static void Save<T>(string path, T value) where T : class, new()
        {
            var serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DefaultValueHandling = DefaultValueHandling.Include
            };

            if (!Directory.Exists(path))
                Directory.CreateDirectory(Path.GetDirectoryName(path) ?? throw new NullReferenceException());

            string valueSerialized = JsonConvert.SerializeObject(value, serializerSettings);

            File.WriteAllText(path, valueSerialized);
        }
    }
}

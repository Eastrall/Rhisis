using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using Rhisis.CLI.Services;

namespace Rhisis.CLI.Core
{
    /// <summary>
    /// Provides a mechanism to fill an object using the console inputs.
    /// </summary>
    /// <typeparam name="TObject">Configuration class.</typeparam>
    internal sealed class ObjectConfigurationFiller<TObject> where TObject : class, new()
    {
        private readonly ConsoleHelper _consoleHelper;

        private List<ObjectConfigurationPropertyInfo> _properties;

        /// <summary>
        /// Gets the current <typeparamref name="TObject"/> value.
        /// </summary>
        public TObject Value { get; }

        /// <summary>
        /// Creates a new <see cref="ObjectConfigurationFiller{TObject}"/> instance.
        /// </summary>
        public ObjectConfigurationFiller()
            : this(null)
        {
        }

        /// <summary>
        /// Creates a new <see cref="ObjectConfigurationFiller{TObject}"/> instance based on an existing instance of <typeparamref name="TObject"/>.
        /// </summary>
        /// <param name="existingInstance">Existing object instance.</param>
        public ObjectConfigurationFiller(TObject existingInstance)
        {
            Value = existingInstance ?? new TObject();
            _consoleHelper = new ConsoleHelper();
            _properties = new List<ObjectConfigurationPropertyInfo>();
            LoadProperties();
        }

        /// <summary>
        /// Fill the object configuration properties.
        /// </summary>
        public void Fill()
        {
            foreach (ObjectConfigurationPropertyInfo objectProperty in _properties)
            {
                Console.Write(objectProperty.Display);

                switch (objectProperty.Type)
                {
                    case ObjectPropertyType.String:
                        objectProperty.Value = objectProperty.IsPassword ?
                            _consoleHelper.ReadPassword() :
                            _consoleHelper.ReadStringOrDefault(objectProperty.Value?.ToString());
                        break;
                    case ObjectPropertyType.Number:
                        objectProperty.Value = _consoleHelper.ReadIntegerOrDefault(Convert.ToInt32(objectProperty.Value));
                        break;
                    case ObjectPropertyType.YesNo:
                        objectProperty.Value = _consoleHelper.AskConfirmation();
                        break;
                    case ObjectPropertyType.Version:
                        string versionInput = _consoleHelper.ReadStringOrDefault(objectProperty.Value?.ToString());

                        if (!Version.TryParse(versionInput, out Version version))
                        {
                            throw new InvalidOperationException($"Failed to parse version: '{versionInput}'");
                        }

                        objectProperty.Value = version;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// Shows the object properties with their valeues.
        /// </summary>
        /// <param name="title">Title to display has header.</param>
        public void Show(string title = null)
        {
            if (!string.IsNullOrEmpty(title))
            {
                Console.WriteLine($"---------- {title} ----------");
            }

            foreach (ObjectConfigurationPropertyInfo property in _properties.Where(property => !property.IsPassword))
            {
                Console.WriteLine($"{property.DisplayName} = {property.Value}");
            }
        }

        /// <summary>
        /// Loads the object properties as <see cref="ObjectConfigurationPropertyInfo"/> instances.
        /// </summary>
        private void LoadProperties()
        {
            IEnumerable<PropertyInfo> objectProperties = typeof(TObject).GetProperties().Where(x => (x.PropertyType.IsPrimitive && x.PropertyType.IsValueType) || (x.PropertyType.Name == typeof(string).Name));

            foreach (PropertyInfo property in objectProperties)
            {
                if (property.GetCustomAttribute<NotMappedAttribute>() != null)
                    continue;

                string propertyDisplayName = property.Name;
                string propertyDisplayDescription = string.Empty;
                int propertyDisplayOrder = 0;
                bool propertyIsPassword = property.GetCustomAttribute<PasswordPropertyTextAttribute>() != null ? true : false;

                var displayAttribute = property.GetCustomAttribute<DisplayAttribute>();

                if (displayAttribute != null)
                {
                    propertyDisplayName = displayAttribute.Name;
                    propertyDisplayDescription = displayAttribute.Description;
                    propertyDisplayOrder = displayAttribute.Order;
                }

                var objectProperty = new ObjectConfigurationPropertyInfo(propertyDisplayName, 
                    propertyDisplayOrder, 
                    propertyIsPassword, 
                    Value, 
                    property);

                if (objectProperty.IsNullOrDefault)
                {
                    var defaultValueAttribute = property.GetCustomAttribute<DefaultValueAttribute>();

                    if (defaultValueAttribute != null)
                    {
                        objectProperty.Value = defaultValueAttribute.Value;
                    }
                }

                _properties.Add(objectProperty);
            }

            _properties = _properties.OrderBy(x => x.Order).ToList();
        }

    }
}

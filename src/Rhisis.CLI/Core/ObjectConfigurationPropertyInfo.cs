using System;
using System.Reflection;
using System.Text;

namespace Rhisis.CLI.Core
{
    /// <summary>
    /// Represents an object configuration property.
    /// </summary>
    internal sealed class ObjectConfigurationPropertyInfo
    {
        private readonly object _instance;
        private readonly PropertyInfo _propertyInfo;

        /// <summary>
        /// Gets a value that indicates if the property is a password.
        /// </summary>
        public bool IsPassword { get; }

        /// <summary>
        /// Gets the property display order.
        /// </summary>
        public int Order { get; }

        /// <summary>
        /// Gets the property display name.
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Gets the property display format.
        /// </summary>
        public string Display
        {
            get
            {
                var builder = new StringBuilder();

                builder.Append(DisplayName);

                if (Value != null && Type != ObjectPropertyType.YesNo && !IsPassword)
                {
                    builder.Append($" ({Value})");
                }

                if (Type == ObjectPropertyType.YesNo)
                {
                    builder.Append($" (y/n)");
                }

                builder.Append(": ");

                return builder.ToString();
            }
        }

        public ObjectPropertyType Type { get; set; }

        public object Value
        {
            get => _propertyInfo.GetValue(_instance);
            set => _propertyInfo.SetValue(_instance, value);
        }

        public bool IsNullOrDefault => Value?.Equals(GetDefault()) ?? true;

        public ObjectConfigurationPropertyInfo(string displayName, int order, bool isPassword, object instance, PropertyInfo propertyInfo)
        {
            DisplayName = displayName;
            Order = order;
            IsPassword = isPassword;
            _instance = instance;
            _propertyInfo = propertyInfo;

            if (propertyInfo.PropertyType == typeof(bool))
                Type = ObjectPropertyType.YesNo;
            else if (propertyInfo.PropertyType == typeof(string))
                Type = ObjectPropertyType.String;
            else if (propertyInfo.PropertyType == typeof(Version))
                Type = ObjectPropertyType.Version;
            else
                Type = ObjectPropertyType.Number;
        }


        private object GetDefault() => _propertyInfo.PropertyType.IsValueType ? Activator.CreateInstance(_propertyInfo.PropertyType) : null;
    }
}

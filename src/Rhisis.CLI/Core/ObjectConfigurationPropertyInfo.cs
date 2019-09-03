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

                builder.Append(this.DisplayName);

                if (this.Value != null && this.Type != ObjectPropertyType.YES_NO)
                {
                    builder.Append($" ({this.Value})");
                }

                if (this.Type == ObjectPropertyType.YES_NO)
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
            get => this._propertyInfo.GetValue(this._instance);
            set => this._propertyInfo.SetValue(this._instance, value);
        }

        public bool IsNullOrDefault => this.Value?.Equals(this.GetDefault()) ?? true;

        public ObjectConfigurationPropertyInfo(string displayName, int order, bool isPassword, object instance, PropertyInfo propertyInfo)
        {
            this.DisplayName = displayName;
            this.Order = order;
            this.IsPassword = isPassword;
            this._instance = instance;
            this._propertyInfo = propertyInfo;

            if (propertyInfo.PropertyType == typeof(bool))
                this.Type = ObjectPropertyType.YES_NO;
            else if (propertyInfo.PropertyType == typeof(string))
                this.Type = ObjectPropertyType.STRING;
            else
                this.Type = ObjectPropertyType.NUMBER;
        }


        private object GetDefault() => this._propertyInfo.PropertyType.IsValueType ? Activator.CreateInstance(this._propertyInfo.PropertyType) : null;
    }
}

using System;
using System.Globalization;

namespace Rhisis.ServerManager.Converters
{
    public class BooleanToEnumConverter : BooleanToStringConverter
    {
        public Type Type { get; set; }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var stringValue = (string)base.Convert(value, targetType, parameter, culture);
            if (stringValue != null) return Enum.Parse(Type, stringValue);
            return null;
        }
    }
}
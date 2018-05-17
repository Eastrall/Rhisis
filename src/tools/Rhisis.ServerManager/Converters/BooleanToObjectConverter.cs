using Catel.MVVM.Converters;
using System;

namespace Rhisis.ServerManager.Converters
{
    public class BooleanToObjectConverter<TObject> : ValueConverterBase
    {
        public TObject TrueValue { get; set; }
        public TObject FalseValue { get; set; }
        
        protected override object Convert(object value, Type targetType, object parameter)
        {
            if (!(value is bool))
                return null;
            return (bool)value ? TrueValue : FalseValue;
        }

        protected override object ConvertBack(object value, Type targetType, object parameter)
        {
            if (Equals(value, TrueValue))
                return true;
            if (Equals(value, FalseValue))
                return false;
            return null;
        }
    }
}
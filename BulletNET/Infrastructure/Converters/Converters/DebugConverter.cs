using Pallet.Infrastructure.Converters.Converters.Base;

namespace BulletNET.Infrastructure.Converters.Converters
{
    internal class DebugConverter : Converter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Debugger.Break();
            return value;
        }
    }
}
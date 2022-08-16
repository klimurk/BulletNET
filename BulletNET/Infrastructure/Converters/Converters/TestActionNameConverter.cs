using Pallet.Infrastructure.Converters.Converters.Base;

namespace BulletNET.Infrastructure.Converters.Converters;

[ValueConversion(typeof(string), typeof(string))]
internal class TestActionNameConverter : Converter
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string name) return "";
        if (name.Contains("Test Voltage")) return "Voltage";
        if (name.Contains("Test Current")) return "Current";
        if (name.Contains("Test Frequency")) return "Frequency";
        if (name.Contains("firmware")) return "Firmware";
        if (name.Contains("Bluetooth")) return "Bluetooth";
        return "";
    }
}

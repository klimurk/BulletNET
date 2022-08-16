using BulletNET.Services.BarcodeReader.Interface;
using Pallet.Infrastructure.Converters.Converters.Base;

namespace BulletNET.Infrastructure.Converters.Converters;

[ValueConversion(typeof(string), typeof(bool))]
internal class RadarBoardConverter : Converter
{
    private IBarcodeCRC _IBarcodeCRC;

    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        _IBarcodeCRC ??= App.Services.GetService(typeof(IBarcodeCRC)) as IBarcodeCRC;
        return ((string)value).Length == 12 && long.TryParse((string)value, out long sn) && _IBarcodeCRC.IsAllCRCOk(sn);
    }
}
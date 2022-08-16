using BulletNET.EntityFramework.Entities.Users;
using Pallet.Infrastructure.Converters.Converters.Base;

namespace BulletNET.Infrastructure.Converters.Converters;

[ValueConversion(typeof(bool), null)]
internal class UserConverter : Converter
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (User)value == null;
}
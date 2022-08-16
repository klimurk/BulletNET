namespace Pallet.Infrastructure.Converters.Converters.Base;

/// <summary>
/// Base converter.
/// </summary>
internal abstract class Converter : IValueConverter
{
    /// <summary>
    /// Converts the.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="targetType">The target type.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="culture">The culture.</param>
    /// <returns>An object.</returns>
    public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);

    /// <summary>
    /// Converts the back.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="targetType">The target type.</param>
    /// <param name="parameter">The parameter.</param>
    /// <param name="culture">The culture.</param>
    /// <returns>An object.</returns>
    public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException("Back convertation is not supported");
}
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ezRclone.Converters;

/// <summary>
/// Collapses the target when the value is null, or when it is not null if <see cref="Invert"/> is set.
/// </summary>
public sealed class NullToVisibilityConverter : IValueConverter
{
    public bool Invert { get; set; }

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        (value != null) ^ Invert ? Visibility.Visible : Visibility.Collapsed;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}

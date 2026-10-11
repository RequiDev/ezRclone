using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using ezRclone.Core;

namespace ezRclone.Converters;

public sealed class StatusToBrushConverter : IValueConverter
{
    private static readonly Brush Mounted = Freeze(new SolidColorBrush(Color.FromRgb(0x2E, 0xA0, 0x43)));
    private static readonly Brush Busy = Freeze(new SolidColorBrush(Color.FromRgb(0xD2, 0x99, 0x22)));
    private static readonly Brush Failed = Freeze(new SolidColorBrush(Color.FromRgb(0xE5, 0x48, 0x4D)));
    private static readonly Brush Unmounted = Freeze(new SolidColorBrush(Color.FromRgb(0x8B, 0x94, 0x9E)));

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value switch
    {
        MountStatus.Mounted => Mounted,
        MountStatus.Mounting or MountStatus.Unmounting => Busy,
        MountStatus.Failed => Failed,
        _ => Unmounted
    };

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();

    private static Brush Freeze(Brush brush)
    {
        brush.Freeze();
        return brush;
    }
}

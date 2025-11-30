using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ErpCrm.Desktop.Converters;

/// <summary>
/// String değerini Visibility'e çevirir
/// Boş veya null string için Collapsed, değer varsa Visible döner
/// </summary>
public class StringToVisibilityConverter : IValueConverter
{
    /// <summary>
    /// String'den Visibility'e çevirir
    /// </summary>
    /// <param name="value">String değer</param>
    /// <param name="targetType">Hedef tip</param>
    /// <param name="parameter">"Invert" parametresi ile ters çevrilir</param>
    /// <param name="culture">Kültür bilgisi</param>
    /// <returns>Visibility değeri</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool hasValue = !string.IsNullOrWhiteSpace(value as string);

        // Invert parametresi
        if (parameter?.ToString() == "Invert")
        {
            hasValue = !hasValue;
        }

        return hasValue ? Visibility.Visible : Visibility.Collapsed;
    }

    /// <summary>
    /// Visibility'den String'e çevirir (geri dönüşüm - desteklenmiyor)
    /// </summary>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException("StringToVisibilityConverter geri dönüşüm desteklemiyor.");
    }
}
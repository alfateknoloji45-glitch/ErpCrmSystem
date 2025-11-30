using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ErpCrm.Desktop.Converters;

/// <summary>
/// Bool değerini Visibility'e çevirir
/// </summary>
public class BoolToVisibilityConverter : IValueConverter
{
    /// <summary>
    /// Bool'dan Visibility'e çevirir
    /// </summary>
    /// <param name="value">Bool değer</param>
    /// <param name="targetType">Hedef tip</param>
    /// <param name="parameter">"Invert" veya "Negative" parametresi</param>
    /// <param name="culture">Kültür bilgisi</param>
    /// <returns>Visibility değeri</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool boolValue = false;
        
        if (value is bool b)
        {
            boolValue = b;
        }
        else if (value is decimal d)
        {
            // Negative parametresi için negatif değer kontrolü
            if (parameter?.ToString() == "Negative")
            {
                return d < 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            boolValue = d != 0;
        }
        else if (value is int i)
        {
            boolValue = i != 0;
        }

        // Invert parametresi
        if (parameter?.ToString() == "Invert")
        {
            boolValue = !boolValue;
        }

        return boolValue ? Visibility.Visible : Visibility.Collapsed;
    }

    /// <summary>
    /// Visibility'den Bool'a çevirir (geri dönüşüm)
    /// </summary>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Visibility visibility)
        {
            bool result = visibility == Visibility.Visible;
            
            if (parameter?.ToString() == "Invert")
            {
                result = !result;
            }
            
            return result;
        }
        
        return false;
    }
}
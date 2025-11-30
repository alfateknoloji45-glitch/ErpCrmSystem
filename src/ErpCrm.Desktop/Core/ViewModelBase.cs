using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ErpCrm.Desktop.Core;

/// <summary>
/// ViewModel'ler için temel sınıf
/// INotifyPropertyChanged implementasyonu içerir
/// </summary>
public class ViewModelBase : INotifyPropertyChanged
{
    /// <summary>
    /// Property değişiklik olayı
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Property değişikliğini bildirir
    /// </summary>
    /// <param name="propertyName">Değişen property adı</param>
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Property değerini günceller ve değişiklik bildirimi yapar
    /// </summary>
    /// <typeparam name="T">Property tipi</typeparam>
    /// <param name="field">Mevcut değer referansı</param>
    /// <param name="value">Yeni değer</param>
    /// <param name="propertyName">Property adı</param>
    /// <returns>Değer değiştiyse true</returns>
    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
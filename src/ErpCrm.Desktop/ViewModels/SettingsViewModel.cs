using ErpCrm.Desktop.Core;

namespace ErpCrm.Desktop.ViewModels;

/// <summary>
/// Ayarlar ViewModel'i
/// </summary>
public class SettingsViewModel : ViewModelBase
{
    private string _firmaAdi = string.Empty;
    private string _email = string.Empty;
    private string _telefon = string.Empty;
    private string _adres = string.Empty;
    private bool _isLoading;
    private string _message = string.Empty;

    /// <summary>
    /// Firma adı
    /// </summary>
    public string FirmaAdi
    {
        get => _firmaAdi;
        set => SetProperty(ref _firmaAdi, value);
    }

    /// <summary>
    /// Email
    /// </summary>
    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }

    /// <summary>
    /// Telefon
    /// </summary>
    public string Telefon
    {
        get => _telefon;
        set => SetProperty(ref _telefon, value);
    }

    /// <summary>
    /// Adres
    /// </summary>
    public string Adres
    {
        get => _adres;
        set => SetProperty(ref _adres, value);
    }

    /// <summary>
    /// Yükleme durumu
    /// </summary>
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    /// <summary>
    /// Mesaj
    /// </summary>
    public string Message
    {
        get => _message;
        set => SetProperty(ref _message, value);
    }

    /// <summary>
    /// Yeni SettingsViewModel oluşturur
    /// </summary>
    public SettingsViewModel()
    {
        LoadSettings();
    }

    /// <summary>
    /// Ayarları yükler
    /// </summary>
    private void LoadSettings()
    {
        var user = App.CurrentUser;
        if (user != null)
        {
            FirmaAdi = user.FirmaAdi;
            Email = user.Email;
        }
        
        Message = "Ayarlar başarıyla yüklendi.";
    }
}

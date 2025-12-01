using System.Windows.Input;
using ErpCrm.Desktop.Core;

namespace ErpCrm.Desktop.ViewModels;

/// <summary>
/// Ayarlar ViewModel'i
/// </summary>
public class SettingsViewModel : ViewModelBase
{
    private string _adSoyad = string.Empty;
    private string _email = string.Empty;
    private string _firmaAdi = string.Empty;
    private string _rol = string.Empty;
    private bool _demoMu;
    private string? _demoBilgisi;
    private string _currentPassword = string.Empty;
    private string _newPassword = string.Empty;
    private string _confirmPassword = string.Empty;
    private string _errorMessage = string.Empty;
    private string _successMessage = string.Empty;

    /// <summary>
    /// Kullanıcı adı soyadı
    /// </summary>
    public string AdSoyad
    {
        get => _adSoyad;
        set => SetProperty(ref _adSoyad, value);
    }

    /// <summary>
    /// Kullanıcı email adresi
    /// </summary>
    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }

    /// <summary>
    /// Firma adı
    /// </summary>
    public string FirmaAdi
    {
        get => _firmaAdi;
        set => SetProperty(ref _firmaAdi, value);
    }

    /// <summary>
    /// Kullanıcı rolü
    /// </summary>
    public string Rol
    {
        get => _rol;
        set => SetProperty(ref _rol, value);
    }

    /// <summary>
    /// Demo hesap mı?
    /// </summary>
    public bool DemoMu
    {
        get => _demoMu;
        set => SetProperty(ref _demoMu, value);
    }

    /// <summary>
    /// Demo bilgisi
    /// </summary>
    public string? DemoBilgisi
    {
        get => _demoBilgisi;
        set => SetProperty(ref _demoBilgisi, value);
    }

    /// <summary>
    /// Mevcut şifre
    /// </summary>
    public string CurrentPassword
    {
        get => _currentPassword;
        set => SetProperty(ref _currentPassword, value);
    }

    /// <summary>
    /// Yeni şifre
    /// </summary>
    public string NewPassword
    {
        get => _newPassword;
        set => SetProperty(ref _newPassword, value);
    }

    /// <summary>
    /// Yeni şifre onayı
    /// </summary>
    public string ConfirmPassword
    {
        get => _confirmPassword;
        set => SetProperty(ref _confirmPassword, value);
    }

    /// <summary>
    /// Hata mesajı
    /// </summary>
    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    /// <summary>
    /// Başarı mesajı
    /// </summary>
    public string SuccessMessage
    {
        get => _successMessage;
        set => SetProperty(ref _successMessage, value);
    }

    /// <summary>
    /// Şifre değiştir komutu
    /// </summary>
    public ICommand ChangePasswordCommand { get; }

    /// <summary>
    /// Yeni SettingsViewModel oluşturur
    /// </summary>
    public SettingsViewModel()
    {
        ChangePasswordCommand = new RelayCommand(ExecuteChangePassword);

        // Kullanıcı bilgilerini yükle
        LoadUserInfo();
    }

    /// <summary>
    /// Kullanıcı bilgilerini yükler
    /// </summary>
    private void LoadUserInfo()
    {
        var user = App.CurrentUser;
        if (user != null)
        {
            AdSoyad = user.AdSoyad;
            Email = user.Email;
            FirmaAdi = user.FirmaAdi;
            Rol = user.Rol;
            DemoMu = user.DemoMu;

            if (user.DemoMu && user.DemoBitisTarihi.HasValue)
            {
                var kalanGun = (user.DemoBitisTarihi.Value - DateTime.Now).Days;
                DemoBilgisi = $"Demo süresi: {kalanGun} gün kaldı";
            }
        }
    }

    /// <summary>
    /// Şifre değiştirme işlemi
    /// </summary>
    private void ExecuteChangePassword()
    {
        ErrorMessage = string.Empty;
        SuccessMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(CurrentPassword))
        {
            ErrorMessage = "Mevcut şifre gereklidir.";
            return;
        }

        if (string.IsNullOrWhiteSpace(NewPassword))
        {
            ErrorMessage = "Yeni şifre gereklidir.";
            return;
        }

        if (NewPassword.Length < 6)
        {
            ErrorMessage = "Yeni şifre en az 6 karakter olmalıdır.";
            return;
        }

        if (NewPassword != ConfirmPassword)
        {
            ErrorMessage = "Yeni şifre ve onayı eşleşmiyor.";
            return;
        }

        // TODO: API'ye şifre değiştirme isteği gönderilecek
        System.Windows.MessageBox.Show("Şifre değiştirme özelliği yakında eklenecek.", "Bilgi");
    }
}

using System.Windows.Input;
using ErpCrm.Desktop.Core;
using ErpCrm.Desktop.Models;

namespace ErpCrm.Desktop.ViewModels;

/// <summary>
/// Login ekranı ViewModel'i
/// </summary>
public class LoginViewModel : ViewModelBase
{
    private string _email = string.Empty;
    private string _password = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isLoading;

    /// <summary>
    /// Email adresi
    /// </summary>
    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }

    /// <summary>
    /// Şifre (binding için - gerçek değer code-behind'dan alınır)
    /// </summary>
    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
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
    /// Yükleme durumu
    /// </summary>
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    /// <summary>
    /// Login komutu
    /// </summary>
    public ICommand LoginCommand { get; }

    /// <summary>
    /// Login başarılı olduğunda tetiklenen event
    /// </summary>
    public event EventHandler<LoginResponse>? LoginSuccess;

    /// <summary>
    /// Yeni LoginViewModel oluşturur
    /// </summary>
    public LoginViewModel()
    {
        LoginCommand = new AsyncRelayCommand(ExecuteLoginAsync, CanExecuteLogin);

        // Demo bilgileri için varsayılan değerler
        Email = "admin@demofirma.com";
    }

    /// <summary>
    /// Login işlemi yapılabilir mi?
    /// </summary>
    private bool CanExecuteLogin(object? parameter)
    {
        return !IsLoading && 
               !string.IsNullOrWhiteSpace(Email) && 
               !string.IsNullOrWhiteSpace(Password);
    }

    /// <summary>
    /// Login işlemini gerçekleştirir
    /// </summary>
    private async Task ExecuteLoginAsync(object? parameter)
    {
        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            // Validasyon
            if (string.IsNullOrWhiteSpace(Email))
            {
                ErrorMessage = "Email adresi gereklidir.";
                return;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Şifre gereklidir.";
                return;
            }

            // API çağrısı
            var response = await App.ApiService.LoginAsync(Email, Password);

            if (response != null)
            {
                // Auth bilgilerini kaydet
                App.ApiService.SetAuth(response.Token, response.TenantId);
                App.CurrentUser = response;

                // Başarı event'ini tetikle
                LoginSuccess?.Invoke(this, response);
            }
            else
            {
                ErrorMessage = "Email veya şifre hatalı!";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Bağlantı hatası: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Şifreyi günceller (PasswordBox için)
    /// </summary>
    /// <param name="password">Yeni şifre</param>
    public void UpdatePassword(string password)
    {
        Password = password;
    }
}
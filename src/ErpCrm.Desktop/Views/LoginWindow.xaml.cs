using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ErpCrm.Desktop.ViewModels;

namespace ErpCrm.Desktop.Views;

/// <summary>
/// LoginWindow.xaml için kod arkası
/// </summary>
public partial class LoginWindow : Window
{
    private readonly LoginViewModel _viewModel;

    public LoginWindow()
    {
        InitializeComponent();

        // ViewModel'e erişim
        _viewModel = (LoginViewModel)DataContext;
        
        // Login başarılı olduğunda MainWindow'a geç
        _viewModel.LoginSuccess += OnLoginSuccess;
    }

    /// <summary>
    /// PasswordBox değişikliğini ViewModel'e bildirir
    /// WPF'de PasswordBox binding desteklemediği için code-behind gerekli
    /// </summary>
    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (sender is PasswordBox passwordBox)
        {
            _viewModel.UpdatePassword(passwordBox.Password);
        }
    }

    /// <summary>
    /// PasswordBox KeyDown event handler - Enter tuşu ile giriş yapma
    /// </summary>
    private void PasswordBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && _viewModel.LoginCommand.CanExecute(null))
        {
            _viewModel.LoginCommand.Execute(null);
        }
    }

    /// <summary>
    /// Login başarılı olduğunda çağrılır
    /// </summary>
    private void OnLoginSuccess(object? sender, Models.LoginResponse response)
    {
        // MainWindow'u aç
        var mainWindow = new MainWindow();
        mainWindow.Show();

        // LoginWindow'u kapat
        Close();
    }

    /// <summary>
    /// Pencere kapatıldığında event'leri temizle
    /// </summary>
    protected override void OnClosed(EventArgs e)
    {
        _viewModel.LoginSuccess -= OnLoginSuccess;
        base.OnClosed(e);
    }
}
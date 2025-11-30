using System.Windows;
using ErpCrm.Desktop.ViewModels;

namespace ErpCrm.Desktop.Views;

/// <summary>
/// MainWindow.xaml için kod arkası
/// </summary>
public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;

    public MainWindow()
    {
        InitializeComponent();

        // ViewModel'e erişim
        _viewModel = (MainViewModel)DataContext;
        
        // Logout olduğunda LoginWindow'a geri dön
        _viewModel.LogoutRequested += OnLogoutRequested;
    }

    /// <summary>
    /// Logout yapıldığında çağrılır
    /// </summary>
    private void OnLogoutRequested(object? sender, EventArgs e)
    {
        // LoginWindow'u aç
        var loginWindow = new LoginWindow();
        loginWindow.Show();

        // MainWindow'u kapat
        Close();
    }

    /// <summary>
    /// Pencere kapatıldığında event'leri temizle
    /// </summary>
    protected override void OnClosed(EventArgs e)
    {
        _viewModel.LogoutRequested -= OnLogoutRequested;
        base.OnClosed(e);
    }
}
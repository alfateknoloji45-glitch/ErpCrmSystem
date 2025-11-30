using System.Windows;

namespace ErpCrm.Desktop;

/// <summary>
/// App.xaml için kod arkası
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Uygulama genelinde kullanılacak API servisi
    /// </summary>
    public static Services.ApiService ApiService { get; } = new Services.ApiService();
    
    /// <summary>
    /// Login yanıtı (oturum bilgisi)
    /// </summary>
    public static Models.LoginResponse? CurrentUser { get; set; }
}
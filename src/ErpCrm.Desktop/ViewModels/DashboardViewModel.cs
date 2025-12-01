using ErpCrm.Desktop.Core;

namespace ErpCrm.Desktop.ViewModels;

/// <summary>
/// Dashboard ViewModel'i - Ana sayfa istatistiklerini gösterir
/// </summary>
public class DashboardViewModel : ViewModelBase
{
    private int _toplamMusteri;
    private int _toplamUrun;
    private int _toplamFatura;
    private bool _isLoading;
    private string _errorMessage = string.Empty;

    /// <summary>
    /// Toplam müşteri sayısı
    /// </summary>
    public int ToplamMusteri
    {
        get => _toplamMusteri;
        set => SetProperty(ref _toplamMusteri, value);
    }

    /// <summary>
    /// Toplam ürün sayısı
    /// </summary>
    public int ToplamUrun
    {
        get => _toplamUrun;
        set => SetProperty(ref _toplamUrun, value);
    }

    /// <summary>
    /// Toplam fatura sayısı
    /// </summary>
    public int ToplamFatura
    {
        get => _toplamFatura;
        set => SetProperty(ref _toplamFatura, value);
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
    /// Hata mesajı
    /// </summary>
    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    /// <summary>
    /// Yeni DashboardViewModel oluşturur
    /// </summary>
    public DashboardViewModel()
    {
        // Fire and forget initialization - exceptions are handled internally
        _ = InitializeAsync();
    }

    /// <summary>
    /// Dashboard verilerini async olarak yükler
    /// </summary>
    private async Task InitializeAsync()
    {
        await LoadDashboardAsync();
    }

    /// <summary>
    /// Dashboard verilerini API'den yükler
    /// </summary>
    private async Task LoadDashboardAsync()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            var stats = await App.ApiService.GetAsync<DashboardStats>("dashboard/stats");
            if (stats != null)
            {
                ToplamMusteri = stats.ToplamMusteri;
                ToplamUrun = stats.ToplamUrun;
                ToplamFatura = stats.ToplamFatura;
            }
            else
            {
                // If API fails, try to get counts from individual endpoints
                await LoadStatsFromIndividualEndpointsAsync();
            }
        }
        catch
        {
            // Set defaults if API fails
            await LoadStatsFromIndividualEndpointsAsync();
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Stats API yoksa bireysel endpoint'lerden verileri yükler
    /// </summary>
    private async Task LoadStatsFromIndividualEndpointsAsync()
    {
        try
        {
            // Try to get customer count
            var cariler = await App.ApiService.GetAsync<List<Models.Cari>>("cari");
            ToplamMusteri = cariler?.Count ?? 0;

            // Try to get product count
            var stoklar = await App.ApiService.GetAsync<List<Models.StokKarti>>("stok");
            ToplamUrun = stoklar?.Count ?? 0;

            // Try to get invoice count
            var faturalar = await App.ApiService.GetAsync<List<object>>("fatura");
            ToplamFatura = faturalar?.Count ?? 0;
        }
        catch
        {
            // Set defaults if all fails
            ToplamMusteri = 0;
            ToplamUrun = 0;
            ToplamFatura = 0;
        }
    }
}

/// <summary>
/// Dashboard istatistikleri model sınıfı
/// </summary>
public class DashboardStats
{
    public int ToplamMusteri { get; set; }
    public int ToplamUrun { get; set; }
    public int ToplamFatura { get; set; }
}

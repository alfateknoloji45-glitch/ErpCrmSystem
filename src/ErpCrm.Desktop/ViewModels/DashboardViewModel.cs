using System.Collections.ObjectModel;
using System.Windows.Input;
using ErpCrm.Desktop.Core;
using ErpCrm.Desktop.Models;

namespace ErpCrm.Desktop.ViewModels;

/// <summary>
/// Dashboard (Ana Sayfa) ViewModel'i
/// </summary>
public class DashboardViewModel : ViewModelBase
{
    private int _customerCount;
    private int _productCount;
    private int _invoiceCount;
    private decimal _totalRevenue;
    private int _lowStockCount;
    private ObservableCollection<RecentInvoice> _recentInvoices = new();
    private bool _isLoading;
    private string _errorMessage = string.Empty;

    /// <summary>
    /// Müşteri sayısı
    /// </summary>
    public int CustomerCount
    {
        get => _customerCount;
        set => SetProperty(ref _customerCount, value);
    }

    /// <summary>
    /// Ürün sayısı
    /// </summary>
    public int ProductCount
    {
        get => _productCount;
        set => SetProperty(ref _productCount, value);
    }

    /// <summary>
    /// Fatura sayısı
    /// </summary>
    public int InvoiceCount
    {
        get => _invoiceCount;
        set => SetProperty(ref _invoiceCount, value);
    }

    /// <summary>
    /// Toplam ciro
    /// </summary>
    public decimal TotalRevenue
    {
        get => _totalRevenue;
        set => SetProperty(ref _totalRevenue, value);
    }

    /// <summary>
    /// Düşük stok sayısı
    /// </summary>
    public int LowStockCount
    {
        get => _lowStockCount;
        set => SetProperty(ref _lowStockCount, value);
    }

    /// <summary>
    /// Son faturalar
    /// </summary>
    public ObservableCollection<RecentInvoice> RecentInvoices
    {
        get => _recentInvoices;
        set => SetProperty(ref _recentInvoices, value);
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
    /// Yenile komutu
    /// </summary>
    public ICommand RefreshCommand { get; }

    /// <summary>
    /// Yeni DashboardViewModel oluşturur
    /// </summary>
    public DashboardViewModel()
    {
        RefreshCommand = new AsyncRelayCommand(LoadStatsAsync);

        // İstatistikleri yükle
        _ = LoadStatsAsync();
    }

    /// <summary>
    /// Dashboard istatistiklerini API'den yükler
    /// </summary>
    private async Task LoadStatsAsync(object? parameter = null)
    {
        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            var result = await App.ApiService.GetAsync<DashboardStats>("dashboard/stats");

            if (result != null)
            {
                CustomerCount = result.CustomerCount;
                ProductCount = result.ProductCount;
                InvoiceCount = result.InvoiceCount;
                TotalRevenue = result.TotalRevenue;
                LowStockCount = result.LowStockCount;

                RecentInvoices.Clear();
                if (result.RecentInvoices != null)
                {
                    foreach (var invoice in result.RecentInvoices)
                    {
                        RecentInvoices.Add(invoice);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"İstatistikler yüklenirken hata oluştu: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
}

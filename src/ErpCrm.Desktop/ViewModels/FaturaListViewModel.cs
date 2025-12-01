using System.Collections.ObjectModel;
using System.Windows.Input;
using ErpCrm.Desktop.Core;
using ErpCrm.Desktop.Models;

namespace ErpCrm.Desktop.ViewModels;

/// <summary>
/// Fatura listesi ViewModel'i
/// </summary>
public class FaturaListViewModel : ViewModelBase
{
    private ObservableCollection<Fatura> _faturalar = new();
    private Fatura? _selectedFatura;
    private bool _isLoading;
    private string _errorMessage = string.Empty;

    /// <summary>
    /// Fatura listesi
    /// </summary>
    public ObservableCollection<Fatura> Faturalar
    {
        get => _faturalar;
        set => SetProperty(ref _faturalar, value);
    }

    /// <summary>
    /// Seçili fatura
    /// </summary>
    public Fatura? SelectedFatura
    {
        get => _selectedFatura;
        set => SetProperty(ref _selectedFatura, value);
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
    /// Yeni fatura komutu
    /// </summary>
    public ICommand NewCommand { get; }

    /// <summary>
    /// Görüntüle komutu
    /// </summary>
    public ICommand ViewCommand { get; }

    /// <summary>
    /// Yazdır komutu
    /// </summary>
    public ICommand PrintCommand { get; }

    /// <summary>
    /// Yeni FaturaListViewModel oluşturur
    /// </summary>
    public FaturaListViewModel()
    {
        RefreshCommand = new AsyncRelayCommand(LoadFaturalarAsync);
        NewCommand = new RelayCommand(ExecuteNew);
        ViewCommand = new RelayCommand(ExecuteView, CanExecuteView);
        PrintCommand = new RelayCommand(ExecutePrint, CanExecutePrint);

        // Faturaları yükle
        _ = LoadFaturalarAsync();
    }

    /// <summary>
    /// Faturaları API'den yükler
    /// </summary>
    private async Task LoadFaturalarAsync(object? parameter = null)
    {
        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            var result = await App.ApiService.GetAsync<List<Fatura>>("fatura");

            Faturalar.Clear();
            if (result != null)
            {
                foreach (var fatura in result)
                {
                    Faturalar.Add(fatura);
                }
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Faturalar yüklenirken hata oluştu: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Yeni fatura oluşturur
    /// </summary>
    private void ExecuteNew()
    {
        System.Windows.MessageBox.Show("Yeni fatura oluşturma penceresi açılacak.", "Bilgi");
    }

    /// <summary>
    /// Görüntüle komutu çalıştırılabilir mi?
    /// </summary>
    private bool CanExecuteView(object? parameter)
    {
        return SelectedFatura != null;
    }

    /// <summary>
    /// Fatura görüntüler
    /// </summary>
    private void ExecuteView(object? parameter)
    {
        if (SelectedFatura == null) return;

        System.Windows.MessageBox.Show($"'{SelectedFatura.FaturaNo}' fatura detayları gösterilecek.", "Bilgi");
    }

    /// <summary>
    /// Yazdır komutu çalıştırılabilir mi?
    /// </summary>
    private bool CanExecutePrint(object? parameter)
    {
        return SelectedFatura != null;
    }

    /// <summary>
    /// Fatura yazdırır
    /// </summary>
    private void ExecutePrint(object? parameter)
    {
        if (SelectedFatura == null) return;

        System.Windows.MessageBox.Show($"'{SelectedFatura.FaturaNo}' yazdırılacak.", "Bilgi");
    }
}

using System.Collections.ObjectModel;
using System.Windows.Input;
using ErpCrm.Desktop.Core;

namespace ErpCrm.Desktop.ViewModels;

/// <summary>
/// Fatura model sınıfı (ViewModel için)
/// </summary>
public class FaturaModel
{
    public int Id { get; set; }
    public string FaturaNo { get; set; } = string.Empty;
    public DateTime FaturaTarihi { get; set; }
    public string CariAdi { get; set; } = string.Empty;
    public decimal GenelToplam { get; set; }
    public string Durum { get; set; } = string.Empty;
}

/// <summary>
/// Fatura listesi ViewModel'i
/// </summary>
public class FaturaListViewModel : ViewModelBase
{
    private ObservableCollection<FaturaModel> _faturalar = new();
    private FaturaModel? _selectedFatura;
    private string _searchText = string.Empty;
    private bool _isLoading;
    private string _errorMessage = string.Empty;

    /// <summary>
    /// Fatura listesi
    /// </summary>
    public ObservableCollection<FaturaModel> Faturalar
    {
        get => _faturalar;
        set => SetProperty(ref _faturalar, value);
    }

    /// <summary>
    /// Seçili fatura
    /// </summary>
    public FaturaModel? SelectedFatura
    {
        get => _selectedFatura;
        set => SetProperty(ref _selectedFatura, value);
    }

    /// <summary>
    /// Arama metni
    /// </summary>
    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
            {
                _ = SearchAsync();
            }
        }
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
    /// Düzenle komutu
    /// </summary>
    public ICommand EditCommand { get; }

    /// <summary>
    /// Sil komutu
    /// </summary>
    public ICommand DeleteCommand { get; }

    /// <summary>
    /// Yeni FaturaListViewModel oluşturur
    /// </summary>
    public FaturaListViewModel()
    {
        RefreshCommand = new AsyncRelayCommand(LoadFaturaAsync);
        NewCommand = new RelayCommand(ExecuteNew);
        EditCommand = new RelayCommand(ExecuteEdit, CanExecuteEdit);
        DeleteCommand = new AsyncRelayCommand(ExecuteDeleteAsync, CanExecuteDelete);

        _ = LoadFaturaAsync();
    }

    /// <summary>
    /// Faturaları API'den yükler
    /// </summary>
    private async Task LoadFaturaAsync(object? parameter = null)
    {
        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            var result = await App.ApiService.GetAsync<List<FaturaModel>>("fatura");

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
    /// Fatura arama yapar
    /// </summary>
    private async Task SearchAsync()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                await LoadFaturaAsync();
                return;
            }

            IsLoading = true;
            ErrorMessage = string.Empty;

            var result = await App.ApiService.GetAsync<List<FaturaModel>>($"fatura/search?q={Uri.EscapeDataString(SearchText)}");

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
            ErrorMessage = $"Arama yapılırken hata oluştu: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void ExecuteNew()
    {
        System.Windows.MessageBox.Show("Yeni fatura ekleme penceresi açılacak.", "Bilgi");
    }

    private bool CanExecuteEdit(object? parameter)
    {
        return SelectedFatura != null;
    }

    private void ExecuteEdit(object? parameter)
    {
        if (SelectedFatura == null) return;
        System.Windows.MessageBox.Show($"'{SelectedFatura.FaturaNo}' düzenleme penceresi açılacak.", "Bilgi");
    }

    private bool CanExecuteDelete(object? parameter)
    {
        return SelectedFatura != null;
    }

    private async Task ExecuteDeleteAsync(object? parameter)
    {
        if (SelectedFatura == null) return;

        var result = System.Windows.MessageBox.Show(
            $"'{SelectedFatura.FaturaNo}' silinecek. Emin misiniz?",
            "Silme Onayı",
            System.Windows.MessageBoxButton.YesNo,
            System.Windows.MessageBoxImage.Warning);

        if (result != System.Windows.MessageBoxResult.Yes) return;

        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            var success = await App.ApiService.DeleteAsync($"fatura/{SelectedFatura.Id}");

            if (success)
            {
                Faturalar.Remove(SelectedFatura);
                SelectedFatura = null;
                System.Windows.MessageBox.Show("Fatura başarıyla silindi.", "Bilgi");
            }
            else
            {
                ErrorMessage = "Fatura silinemedi.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Silme işlemi sırasında hata oluştu: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
}

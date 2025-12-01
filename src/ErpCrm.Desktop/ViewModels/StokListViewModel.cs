using System.Collections.ObjectModel;
using System.Windows.Input;
using ErpCrm.Desktop.Core;
using ErpCrm.Desktop.Models;

namespace ErpCrm.Desktop.ViewModels;

/// <summary>
/// Stok listesi ViewModel'i
/// </summary>
public class StokListViewModel : ViewModelBase
{
    private ObservableCollection<StokKarti> _stoklar = new();
    private StokKarti? _selectedStok;
    private string _searchText = string.Empty;
    private bool _isLoading;
    private string _errorMessage = string.Empty;

    /// <summary>
    /// Stok kartları listesi
    /// </summary>
    public ObservableCollection<StokKarti> Stoklar
    {
        get => _stoklar;
        set => SetProperty(ref _stoklar, value);
    }

    /// <summary>
    /// Seçili stok kartı
    /// </summary>
    public StokKarti? SelectedStok
    {
        get => _selectedStok;
        set => SetProperty(ref _selectedStok, value);
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
    /// Yeni stok komutu
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
    /// Yeni StokListViewModel oluşturur
    /// </summary>
    public StokListViewModel()
    {
        RefreshCommand = new AsyncRelayCommand(LoadStokAsync);
        NewCommand = new RelayCommand(ExecuteNew);
        EditCommand = new RelayCommand(ExecuteEdit, CanExecuteEdit);
        DeleteCommand = new AsyncRelayCommand(ExecuteDeleteAsync, CanExecuteDelete);

        _ = LoadStokAsync();
    }

    /// <summary>
    /// Stok kartlarını API'den yükler
    /// </summary>
    private async Task LoadStokAsync(object? parameter = null)
    {
        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            var result = await App.ApiService.GetAsync<List<StokKarti>>("stok");

            Stoklar.Clear();
            if (result != null)
            {
                foreach (var stok in result)
                {
                    Stoklar.Add(stok);
                }
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Stok kartları yüklenirken hata oluştu: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Stok arama yapar
    /// </summary>
    private async Task SearchAsync()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                await LoadStokAsync();
                return;
            }

            IsLoading = true;
            ErrorMessage = string.Empty;

            var result = await App.ApiService.GetAsync<List<StokKarti>>($"stok/search?q={Uri.EscapeDataString(SearchText)}");

            Stoklar.Clear();
            if (result != null)
            {
                foreach (var stok in result)
                {
                    Stoklar.Add(stok);
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
        System.Windows.MessageBox.Show("Yeni stok kartı ekleme penceresi açılacak.", "Bilgi");
    }

    private bool CanExecuteEdit(object? parameter)
    {
        return SelectedStok != null;
    }

    private void ExecuteEdit(object? parameter)
    {
        if (SelectedStok == null) return;
        System.Windows.MessageBox.Show($"'{SelectedStok.StokAdi}' düzenleme penceresi açılacak.", "Bilgi");
    }

    private bool CanExecuteDelete(object? parameter)
    {
        return SelectedStok != null;
    }

    private async Task ExecuteDeleteAsync(object? parameter)
    {
        if (SelectedStok == null) return;

        var result = System.Windows.MessageBox.Show(
            $"'{SelectedStok.StokAdi}' silinecek. Emin misiniz?",
            "Silme Onayı",
            System.Windows.MessageBoxButton.YesNo,
            System.Windows.MessageBoxImage.Warning);

        if (result != System.Windows.MessageBoxResult.Yes) return;

        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            var success = await App.ApiService.DeleteAsync($"stok/{SelectedStok.Id}");

            if (success)
            {
                Stoklar.Remove(SelectedStok);
                SelectedStok = null;
                System.Windows.MessageBox.Show("Stok kartı başarıyla silindi.", "Bilgi");
            }
            else
            {
                ErrorMessage = "Stok kartı silinemedi.";
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

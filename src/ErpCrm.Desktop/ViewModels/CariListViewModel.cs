using System.Collections.ObjectModel;
using System.Windows.Input;
using ErpCrm.Desktop.Core;
using ErpCrm.Desktop.Models;

namespace ErpCrm.Desktop.ViewModels;

/// <summary>
/// Cari listesi ViewModel'i
/// </summary>
public class CariListViewModel : ViewModelBase
{
    private ObservableCollection<Cari> _cariler = new();
    private Cari? _selectedCari;
    private string _searchText = string.Empty;
    private bool _isLoading;
    private string _errorMessage = string.Empty;

    /// <summary>
    /// Cari listesi
    /// </summary>
    public ObservableCollection<Cari> Cariler
    {
        get => _cariler;
        set => SetProperty(ref _cariler, value);
    }

    /// <summary>
    /// Seçili cari
    /// </summary>
    public Cari? SelectedCari
    {
        get => _selectedCari;
        set => SetProperty(ref _selectedCari, value);
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
                // Arama metni değiştiğinde otomatik arama yap
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
    /// Yeni cari komutu
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
    /// Yeni CariListViewModel oluşturur
    /// </summary>
    public CariListViewModel()
    {
        RefreshCommand = new AsyncRelayCommand(LoadCarilerAsync);
        NewCommand = new RelayCommand(ExecuteNew);
        EditCommand = new RelayCommand(ExecuteEdit, CanExecuteEdit);
        DeleteCommand = new AsyncRelayCommand(ExecuteDeleteAsync, CanExecuteDelete);

        // Carileri yükle
        _ = LoadCarilerAsync();
    }

    /// <summary>
    /// Carileri API'den yükler
    /// </summary>
    private async Task LoadCarilerAsync(object? parameter = null)
    {
        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            var result = await App.ApiService.GetAsync<List<Cari>>("cari");

            Cariler.Clear();
            if (result != null)
            {
                foreach (var cari in result)
                {
                    Cariler.Add(cari);
                }
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Cariler yüklenirken hata oluştu: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Cari arama yapar
    /// </summary>
    private async Task SearchAsync()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                await LoadCarilerAsync();
                return;
            }

            IsLoading = true;
            ErrorMessage = string.Empty;

            var result = await App.ApiService.GetAsync<List<Cari>>($"cari/search?q={Uri.EscapeDataString(SearchText)}");

            Cariler.Clear();
            if (result != null)
            {
                foreach (var cari in result)
                {
                    Cariler.Add(cari);
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

    /// <summary>
    /// Yeni cari oluşturur
    /// </summary>
    private void ExecuteNew()
    {
        // TODO: Yeni cari penceresi açılacak
        System.Windows.MessageBox.Show("Yeni cari ekleme penceresi açılacak.", "Bilgi");
    }

    /// <summary>
    /// Düzenle komutu çalıştırılabilir mi?
    /// </summary>
    private bool CanExecuteEdit(object? parameter)
    {
        return SelectedCari != null;
    }

    /// <summary>
    /// Cari düzenler
    /// </summary>
    private void ExecuteEdit(object? parameter)
    {
        if (SelectedCari == null) return;

        // TODO: Cari düzenleme penceresi açılacak
        System.Windows.MessageBox.Show($"'{SelectedCari.CariAdi}' düzenleme penceresi açılacak.", "Bilgi");
    }

    /// <summary>
    /// Sil komutu çalıştırılabilir mi?
    /// </summary>
    private bool CanExecuteDelete(object? parameter)
    {
        return SelectedCari != null;
    }

    /// <summary>
    /// Cari siler
    /// </summary>
    private async Task ExecuteDeleteAsync(object? parameter)
    {
        if (SelectedCari == null) return;

        var result = System.Windows.MessageBox.Show(
            $"'{SelectedCari.CariAdi}' silinecek. Emin misiniz?",
            "Silme Onayı",
            System.Windows.MessageBoxButton.YesNo,
            System.Windows.MessageBoxImage.Warning);

        if (result != System.Windows.MessageBoxResult.Yes) return;

        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            var success = await App.ApiService.DeleteAsync($"cari/{SelectedCari.Id}");

            if (success)
            {
                Cariler.Remove(SelectedCari);
                SelectedCari = null;
                System.Windows.MessageBox.Show("Cari başarıyla silindi.", "Bilgi");
            }
            else
            {
                ErrorMessage = "Cari silinemedi.";
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
using System.Collections.ObjectModel;
using System.Windows.Input;
using ErpCrm.Desktop.Core;
using ErpCrm.Desktop.Models;

namespace ErpCrm.Desktop.ViewModels;

/// <summary>
/// Ana menü öğesi
/// </summary>
public class MenuItem
{
    public string Title { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string ModuleCode { get; set; } = string.Empty;
    public ICommand? Command { get; set; }
}

/// <summary>
/// Ana pencere ViewModel'i
/// </summary>
public class MainViewModel : ViewModelBase
{
    private string _firmaAdi = string.Empty;
    private string _kullaniciAdi = string.Empty;
    private bool _demoMu;
    private string? _demoBilgisi;
    private ViewModelBase? _currentViewModel;
    private ObservableCollection<MenuItem> _menuItems = new();

    /// <summary>
    /// Firma adı
    /// </summary>
    public string FirmaAdi
    {
        get => _firmaAdi;
        set => SetProperty(ref _firmaAdi, value);
    }

    /// <summary>
    /// Kullanıcı adı
    /// </summary>
    public string KullaniciAdi
    {
        get => _kullaniciAdi;
        set => SetProperty(ref _kullaniciAdi, value);
    }

    /// <summary>
    /// Demo hesap mı?
    /// </summary>
    public bool DemoMu
    {
        get => _demoMu;
        set => SetProperty(ref _demoMu, value);
    }

    /// <summary>
    /// Demo bilgisi (kalan gün vb.)
    /// </summary>
    public string? DemoBilgisi
    {
        get => _demoBilgisi;
        set => SetProperty(ref _demoBilgisi, value);
    }

    /// <summary>
    /// Mevcut içerik ViewModel'i
    /// </summary>
    public ViewModelBase? CurrentViewModel
    {
        get => _currentViewModel;
        set => SetProperty(ref _currentViewModel, value);
    }

    /// <summary>
    /// Menü öğeleri
    /// </summary>
    public ObservableCollection<MenuItem> MenuItems
    {
        get => _menuItems;
        set => SetProperty(ref _menuItems, value);
    }

    /// <summary>
    /// Çıkış komutu
    /// </summary>
    public ICommand LogoutCommand { get; }

    /// <summary>
    /// Çıkış yapıldığında tetiklenen event
    /// </summary>
    public event EventHandler? LogoutRequested;

    /// <summary>
    /// Yeni MainViewModel oluşturur
    /// </summary>
    public MainViewModel()
    {
        LogoutCommand = new RelayCommand(ExecuteLogout);

        // Oturum bilgilerini yükle
        LoadUserInfo();
        
        // Menüyü oluştur
        BuildMenu();
    }

    /// <summary>
    /// Kullanıcı bilgilerini yükler
    /// </summary>
    private void LoadUserInfo()
    {
        var user = App.CurrentUser;
        if (user != null)
        {
            FirmaAdi = user.FirmaAdi;
            KullaniciAdi = user.AdSoyad;
            DemoMu = user.DemoMu;

            if (user.DemoMu && user.DemoBitisTarihi.HasValue)
            {
                var kalanGun = (user.DemoBitisTarihi.Value - DateTime.Now).Days;
                DemoBilgisi = $"Demo - {kalanGun} gün kaldı";
            }
        }
    }

    /// <summary>
    /// Menüyü oluşturur
    /// </summary>
    private void BuildMenu()
    {
        MenuItems.Clear();

        var user = App.CurrentUser;
        if (user == null) return;

        var aktifModuller = user.AktifModuller ?? new List<string>();

        // Ana Sayfa (her zaman görünür)
        MenuItems.Add(new MenuItem
        {
            Title = "🏠 Ana Sayfa",
            Icon = "🏠",
            ModuleCode = "HOME",
            Command = new RelayCommand(() => CurrentViewModel = new DashboardViewModel())
        });

        // Cari Modülü
        if (aktifModuller.Contains("CARI"))
        {
            MenuItems.Add(new MenuItem
            {
                Title = "👥 Cari Yönetimi",
                Icon = "👥",
                ModuleCode = "CARI",
                Command = new RelayCommand(() => CurrentViewModel = new CariListViewModel())
            });
        }

        // Stok Modülü
        if (aktifModuller.Contains("STOK"))
        {
            MenuItems.Add(new MenuItem
            {
                Title = "📦 Stok Yönetimi",
                Icon = "📦",
                ModuleCode = "STOK",
                Command = new RelayCommand(() => CurrentViewModel = new StokListViewModel())
            });
        }

        // Fatura Modülü
        if (aktifModuller.Contains("FATURA"))
        {
            MenuItems.Add(new MenuItem
            {
                Title = "📄 Faturalar",
                Icon = "📄",
                ModuleCode = "FATURA",
                Command = new RelayCommand(() => CurrentViewModel = new FaturaListViewModel())
            });
        }

        // POS Modülü
        if (aktifModuller.Contains("POS"))
        {
            MenuItems.Add(new MenuItem
            {
                Title = "🍽️ POS Sistemi",
                Icon = "🍽️",
                ModuleCode = "POS",
                Command = new RelayCommand(() => { })
            });
        }

        // CRM Modülü
        if (aktifModuller.Contains("CRM"))
        {
            MenuItems.Add(new MenuItem
            {
                Title = "📊 CRM",
                Icon = "📊",
                ModuleCode = "CRM",
                Command = new RelayCommand(() => { })
            });
        }

        // Raporlama Modülü
        if (aktifModuller.Contains("RAPORLAMA"))
        {
            MenuItems.Add(new MenuItem
            {
                Title = "📈 Raporlar",
                Icon = "📈",
                ModuleCode = "RAPORLAMA",
                Command = new RelayCommand(() => { })
            });
        }

        // Ayarlar (Admin için)
        if (user.Rol == "TenantAdmin" || user.Rol == "SuperAdmin")
        {
            MenuItems.Add(new MenuItem
            {
                Title = "⚙️ Ayarlar",
                Icon = "⚙️",
                ModuleCode = "SETTINGS",
                Command = new RelayCommand(() => CurrentViewModel = new SettingsViewModel())
            });
        }

        // Varsayılan olarak Dashboard göster
        CurrentViewModel = new DashboardViewModel();
    }

    /// <summary>
    /// Çıkış işlemini gerçekleştirir
    /// </summary>
    private void ExecuteLogout()
    {
        // Auth bilgilerini temizle
        App.ApiService.ClearAuth();
        App.CurrentUser = null;

        // Logout event'ini tetikle
        LogoutRequested?.Invoke(this, EventArgs.Empty);
    }
}
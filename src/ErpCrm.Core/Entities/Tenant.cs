namespace ErpCrm.Core.Entities;

/// <summary>
/// Tenant (Firma) durumlarını tanımlar
/// </summary>
public enum TenantDurum
{
    Pasif = 0,
    Aktif = 1,
    Demo = 2,
    Askida = 3
}

/// <summary>
/// Firma (Tenant) entity sınıfı
/// Multi-tenant yapıdaki firmaları temsil eder
/// </summary>
public class Tenant
{
    public int Id { get; set; }
    
    /// <summary>
    /// Benzersiz firma kodu
    /// </summary>
    public string FirmaKodu { get; set; } = string.Empty;
    
    /// <summary>
    /// Firma adı
    /// </summary>
    public string FirmaAdi { get; set; } = string.Empty;
    
    /// <summary>
    /// Vergi numarası
    /// </summary>
    public string? VergiNo { get; set; }
    
    /// <summary>
    /// Telefon numarası
    /// </summary>
    public string? Telefon { get; set; }
    
    /// <summary>
    /// Email adresi
    /// </summary>
    public string? Email { get; set; }
    
    /// <summary>
    /// Firma adresi
    /// </summary>
    public string? Adres { get; set; }
    
    /// <summary>
    /// Firma durumu (Pasif, Aktif, Demo, Askıda)
    /// </summary>
    public TenantDurum Durum { get; set; } = TenantDurum.Aktif;
    
    /// <summary>
    /// Demo hesabı mı?
    /// </summary>
    public bool DemoMu { get; set; } = false;
    
    /// <summary>
    /// Demo hesap bitiş tarihi
    /// </summary>
    public DateTime? DemoBitisTarihi { get; set; }
    
    /// <summary>
    /// Kayıt oluşturma tarihi
    /// </summary>
    public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;
    
    /// <summary>
    /// Son güncelleme tarihi
    /// </summary>
    public DateTime? GuncellemeTarihi { get; set; }
    
    // Navigation Properties
    public virtual ICollection<User> Users { get; set; } = new List<User>();
    public virtual ICollection<TenantSubscription> Subscriptions { get; set; } = new List<TenantSubscription>();
    public virtual ICollection<TenantModule> TenantModules { get; set; } = new List<TenantModule>();
    public virtual ICollection<Cari> Cariler { get; set; } = new List<Cari>();
    public virtual ICollection<StokKarti> StokKartlari { get; set; } = new List<StokKarti>();
    public virtual ICollection<Fatura> Faturalar { get; set; } = new List<Fatura>();
    public virtual ICollection<Masa> Masalar { get; set; } = new List<Masa>();
    public virtual ICollection<Adisyon> Adisyonlar { get; set; } = new List<Adisyon>();
    public virtual ICollection<CrmMusteri> CrmMusteriler { get; set; } = new List<CrmMusteri>();
    public virtual ICollection<CrmAktivite> CrmAktiviteler { get; set; } = new List<CrmAktivite>();
}
namespace ErpCrm.Core.Entities;

/// <summary>
/// CRM Müşteri entity sınıfı
/// Potansiyel ve mevcut müşteri bilgilerini tutar
/// </summary>
public class CrmMusteri
{
    public int Id { get; set; }
    
    /// <summary>
    /// Firma ID (multi-tenant için)
    /// </summary>
    public int TenantId { get; set; }
    
    /// <summary>
    /// Benzersiz müşteri kodu
    /// </summary>
    public string MusteriKodu { get; set; } = string.Empty;
    
    /// <summary>
    /// Müşteri adı soyadı
    /// </summary>
    public string MusteriAdi { get; set; } = string.Empty;
    
    /// <summary>
    /// Firma adı
    /// </summary>
    public string? FirmaAdi { get; set; }
    
    /// <summary>
    /// Telefon numarası
    /// </summary>
    public string? Telefon { get; set; }
    
    /// <summary>
    /// Email adresi
    /// </summary>
    public string? Email { get; set; }
    
    /// <summary>
    /// Adres
    /// </summary>
    public string? Adres { get; set; }
    
    /// <summary>
    /// İl
    /// </summary>
    public string? Il { get; set; }
    
    /// <summary>
    /// İlçe
    /// </summary>
    public string? Ilce { get; set; }
    
    /// <summary>
    /// Sektör
    /// </summary>
    public string? Sektor { get; set; }
    
    /// <summary>
    /// Müşteri kaynağı (Web, Referans, Reklam vb.)
    /// </summary>
    public string? MusteriKaynagi { get; set; }
    
    /// <summary>
    /// Müşteri durumu (Lead, Prospect, Customer)
    /// </summary>
    public string? MusteriDurumu { get; set; }
    
    /// <summary>
    /// Atanan kullanıcı ID
    /// </summary>
    public int? AtananKullaniciId { get; set; }
    
    /// <summary>
    /// Notlar
    /// </summary>
    public string? Not { get; set; }
    
    /// <summary>
    /// Aktif mi?
    /// </summary>
    public bool Aktif { get; set; } = true;
    
    /// <summary>
    /// Oluşturma tarihi
    /// </summary>
    public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;
    
    /// <summary>
    /// Güncelleme tarihi
    /// </summary>
    public DateTime? GuncellemeTarihi { get; set; }
    
    // Navigation Properties
    public virtual Tenant? Tenant { get; set; }
    public virtual User? AtananKullanici { get; set; }
    public virtual ICollection<CrmAktivite> Aktiviteler { get; set; } = new List<CrmAktivite>();
}
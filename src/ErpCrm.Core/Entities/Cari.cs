namespace ErpCrm.Core.Entities;

/// <summary>
/// Cari tipi (Müşteri, Tedarikçi veya Her İkisi)
/// </summary>
public enum CariTip
{
    Musteri = 0,
    Tedarikci = 1,
    HerIkisi = 2
}

/// <summary>
/// Cari hesap entity sınıfı
/// Müşteri ve tedarikçi bilgilerini tutar
/// </summary>
public class Cari
{
    public int Id { get; set; }
    
    /// <summary>
    /// Firma ID (multi-tenant için)
    /// </summary>
    public int TenantId { get; set; }
    
    /// <summary>
    /// Benzersiz cari kodu
    /// </summary>
    public string CariKodu { get; set; } = string.Empty;
    
    /// <summary>
    /// Cari adı
    /// </summary>
    public string CariAdi { get; set; } = string.Empty;
    
    /// <summary>
    /// Cari tipi (Müşteri, Tedarikçi, Her İkisi)
    /// </summary>
    public CariTip CariTip { get; set; } = CariTip.Musteri;
    
    /// <summary>
    /// Vergi dairesi
    /// </summary>
    public string? VergiDairesi { get; set; }
    
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
    /// Cari bakiye (+ alacak, - borç)
    /// </summary>
    public decimal Bakiye { get; set; } = 0;
    
    /// <summary>
    /// Alacak limiti
    /// </summary>
    public decimal AlacakLimiti { get; set; } = 0;
    
    /// <summary>
    /// Cari aktif mi?
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
    public virtual ICollection<Fatura> Faturalar { get; set; } = new List<Fatura>();
}
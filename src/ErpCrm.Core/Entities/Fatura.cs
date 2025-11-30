namespace ErpCrm.Core.Entities;

/// <summary>
/// Fatura tipi (Alış/Satış)
/// </summary>
public enum FaturaTipi
{
    Alis = 0,
    Satis = 1
}

/// <summary>
/// Fatura durumu
/// </summary>
public enum FaturaDurum
{
    Taslak = 0,
    Onaylandi = 1,
    Iptal = 2
}

/// <summary>
/// Fatura entity sınıfı
/// Alış ve satış faturalarını tutar
/// </summary>
public class Fatura
{
    public int Id { get; set; }
    
    /// <summary>
    /// Firma ID (multi-tenant için)
    /// </summary>
    public int TenantId { get; set; }
    
    /// <summary>
    /// Benzersiz fatura numarası
    /// </summary>
    public string FaturaNo { get; set; } = string.Empty;
    
    /// <summary>
    /// Fatura tarihi
    /// </summary>
    public DateTime FaturaTarihi { get; set; } = DateTime.Now;
    
    /// <summary>
    /// Fatura tipi (Alış/Satış)
    /// </summary>
    public FaturaTipi FaturaTipi { get; set; } = FaturaTipi.Satis;
    
    /// <summary>
    /// İlişkili cari ID
    /// </summary>
    public int? CariId { get; set; }
    
    /// <summary>
    /// Ara toplam (KDV hariç)
    /// </summary>
    public decimal AraToplam { get; set; } = 0;
    
    /// <summary>
    /// KDV toplam tutarı
    /// </summary>
    public decimal KdvToplam { get; set; } = 0;
    
    /// <summary>
    /// İndirim toplam tutarı
    /// </summary>
    public decimal IndirimToplam { get; set; } = 0;
    
    /// <summary>
    /// Genel toplam (KDV dahil)
    /// </summary>
    public decimal GenelToplam { get; set; } = 0;
    
    /// <summary>
    /// Fatura durumu
    /// </summary>
    public FaturaDurum Durum { get; set; } = FaturaDurum.Taslak;
    
    /// <summary>
    /// Açıklama
    /// </summary>
    public string? Aciklama { get; set; }
    
    /// <summary>
    /// Oluşturan kullanıcı ID
    /// </summary>
    public int? OlusturanKullaniciId { get; set; }
    
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
    public virtual Cari? Cari { get; set; }
    public virtual User? OlusturanKullanici { get; set; }
    public virtual ICollection<FaturaSatiri> FaturaSatirlari { get; set; } = new List<FaturaSatiri>();
}
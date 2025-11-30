namespace ErpCrm.Core.Entities;

/// <summary>
/// Stok kartı entity sınıfı
/// Ürün ve hizmet bilgilerini tutar
/// </summary>
public class StokKarti
{
    public int Id { get; set; }
    
    /// <summary>
    /// Firma ID (multi-tenant için)
    /// </summary>
    public int TenantId { get; set; }
    
    /// <summary>
    /// Benzersiz stok kodu
    /// </summary>
    public string StokKodu { get; set; } = string.Empty;
    
    /// <summary>
    /// Stok/Ürün adı
    /// </summary>
    public string StokAdi { get; set; } = string.Empty;
    
    /// <summary>
    /// Barkod numarası
    /// </summary>
    public string? Barkod { get; set; }
    
    /// <summary>
    /// Birim (Adet, Kg, Lt, vb.)
    /// </summary>
    public string Birim { get; set; } = "Adet";
    
    /// <summary>
    /// Ana kategori
    /// </summary>
    public string? Kategori { get; set; }
    
    /// <summary>
    /// Alt kategori
    /// </summary>
    public string? AltKategori { get; set; }
    
    /// <summary>
    /// Alış fiyatı
    /// </summary>
    public decimal AlisFiyati { get; set; } = 0;
    
    /// <summary>
    /// Satış fiyatı
    /// </summary>
    public decimal SatisFiyati { get; set; } = 0;
    
    /// <summary>
    /// KDV oranı (%)
    /// </summary>
    public int KdvOrani { get; set; } = 18;
    
    /// <summary>
    /// Mevcut stok miktarı
    /// </summary>
    public decimal StokMiktari { get; set; } = 0;
    
    /// <summary>
    /// Minimum stok miktarı (uyarı için)
    /// </summary>
    public decimal MinStokMiktari { get; set; } = 0;
    
    /// <summary>
    /// Açıklama
    /// </summary>
    public string? Aciklama { get; set; }
    
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
    public virtual ICollection<FaturaSatiri> FaturaSatirlari { get; set; } = new List<FaturaSatiri>();
    public virtual ICollection<AdisyonSatiri> AdisyonSatirlari { get; set; } = new List<AdisyonSatiri>();
}
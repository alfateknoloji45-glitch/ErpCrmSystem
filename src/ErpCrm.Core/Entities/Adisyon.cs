namespace ErpCrm.Core.Entities;

/// <summary>
/// Adisyon durumu (POS)
/// </summary>
public enum AdisyonDurum
{
    Acik = 0,
    Kapali = 1,
    Iptal = 2
}

/// <summary>
/// Adisyon entity sınıfı (POS)
/// Masa siparişlerini tutar
/// </summary>
public class Adisyon
{
    public int Id { get; set; }
    
    /// <summary>
    /// Firma ID (multi-tenant için)
    /// </summary>
    public int TenantId { get; set; }
    
    /// <summary>
    /// Masa ID
    /// </summary>
    public int MasaId { get; set; }
    
    /// <summary>
    /// Benzersiz adisyon numarası
    /// </summary>
    public string AdisyonNo { get; set; } = string.Empty;
    
    /// <summary>
    /// Adisyon açılış tarihi
    /// </summary>
    public DateTime AcilisTarihi { get; set; } = DateTime.Now;
    
    /// <summary>
    /// Adisyon kapanış tarihi
    /// </summary>
    public DateTime? KapanisTarihi { get; set; }
    
    /// <summary>
    /// Garson ID
    /// </summary>
    public int? GarsonId { get; set; }
    
    /// <summary>
    /// Ara toplam
    /// </summary>
    public decimal AraToplam { get; set; } = 0;
    
    /// <summary>
    /// İndirim toplam
    /// </summary>
    public decimal IndirimToplam { get; set; } = 0;
    
    /// <summary>
    /// Genel toplam
    /// </summary>
    public decimal GenelToplam { get; set; } = 0;
    
    /// <summary>
    /// Ödenen tutar
    /// </summary>
    public decimal OdenenTutar { get; set; } = 0;
    
    /// <summary>
    /// Adisyon durumu
    /// </summary>
    public AdisyonDurum Durum { get; set; } = AdisyonDurum.Acik;
    
    /// <summary>
    /// Açıklama
    /// </summary>
    public string? Aciklama { get; set; }
    
    /// <summary>
    /// Oluşturma tarihi
    /// </summary>
    public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;
    
    // Navigation Properties
    public virtual Tenant? Tenant { get; set; }
    public virtual Masa? Masa { get; set; }
    public virtual User? Garson { get; set; }
    public virtual ICollection<AdisyonSatiri> AdisyonSatirlari { get; set; } = new List<AdisyonSatiri>();
}
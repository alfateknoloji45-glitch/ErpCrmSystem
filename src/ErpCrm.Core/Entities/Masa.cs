namespace ErpCrm.Core.Entities;

/// <summary>
/// Masa durumu (POS)
/// </summary>
public enum MasaDurum
{
    Bos = 0,
    Dolu = 1,
    Rezerve = 2
}

/// <summary>
/// Masa entity sınıfı (POS)
/// Restoran/cafe masalarını tutar
/// </summary>
public class Masa
{
    public int Id { get; set; }
    
    /// <summary>
    /// Firma ID (multi-tenant için)
    /// </summary>
    public int TenantId { get; set; }
    
    /// <summary>
    /// Masa numarası
    /// </summary>
    public string MasaNo { get; set; } = string.Empty;
    
    /// <summary>
    /// Masa adı (isteğe bağlı)
    /// </summary>
    public string? MasaAdi { get; set; }
    
    /// <summary>
    /// Masa kapasitesi (kişi sayısı)
    /// </summary>
    public int Kapasite { get; set; } = 4;
    
    /// <summary>
    /// Bölüm (Salon, Bahçe, VIP vb.)
    /// </summary>
    public string? Bolum { get; set; }
    
    /// <summary>
    /// Masa durumu (Boş, Dolu, Rezerve)
    /// </summary>
    public MasaDurum Durum { get; set; } = MasaDurum.Bos;
    
    /// <summary>
    /// Masa aktif mi?
    /// </summary>
    public bool Aktif { get; set; } = true;
    
    /// <summary>
    /// Oluşturma tarihi
    /// </summary>
    public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;
    
    // Navigation Properties
    public virtual Tenant? Tenant { get; set; }
    public virtual ICollection<Adisyon> Adisyonlar { get; set; } = new List<Adisyon>();
}
namespace ErpCrm.Core.Entities;

/// <summary>
/// CRM Aktivite entity sınıfı
/// Müşteri ile yapılan aktiviteleri (arama, toplantı, email vb.) tutar
/// </summary>
public class CrmAktivite
{
    public int Id { get; set; }
    
    /// <summary>
    /// Firma ID (multi-tenant için)
    /// </summary>
    public int TenantId { get; set; }
    
    /// <summary>
    /// CRM Müşteri ID
    /// </summary>
    public int CrmMusteriId { get; set; }
    
    /// <summary>
    /// Aktivite tipi (Arama, Toplantı, Email, Teklif vb.)
    /// </summary>
    public string AktiviteTipi { get; set; } = string.Empty;
    
    /// <summary>
    /// Aktivite başlığı
    /// </summary>
    public string Baslik { get; set; } = string.Empty;
    
    /// <summary>
    /// Aktivite açıklaması
    /// </summary>
    public string? Aciklama { get; set; }
    
    /// <summary>
    /// Planlanan tarih
    /// </summary>
    public DateTime? PlanlananTarih { get; set; }
    
    /// <summary>
    /// Tamamlanma tarihi
    /// </summary>
    public DateTime? TamamlanmaTarihi { get; set; }
    
    /// <summary>
    /// Durum (Planlandı, Tamamlandı, İptal)
    /// </summary>
    public string Durum { get; set; } = "Planlandı";
    
    /// <summary>
    /// Sorumlu kullanıcı ID
    /// </summary>
    public int? SorumluKullaniciId { get; set; }
    
    /// <summary>
    /// Öncelik (1: Düşük, 2: Normal, 3: Yüksek)
    /// </summary>
    public int Oncelik { get; set; } = 2;
    
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
    public virtual CrmMusteri? CrmMusteri { get; set; }
    public virtual User? SorumluKullanici { get; set; }
}
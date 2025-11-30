namespace ErpCrm.Core.Entities;

/// <summary>
/// Ödeme tipi (Aylık/Yıllık)
/// </summary>
public enum OdemeTipi
{
    Aylik = 1,
    Yillik = 2
}

/// <summary>
/// Abonelik durumu
/// </summary>
public enum SubscriptionDurum
{
    Pasif = 0,
    Aktif = 1,
    SuresiDolmus = 2
}

/// <summary>
/// Firma abonelik entity sınıfı
/// Firmaların abonelik bilgilerini tutar
/// </summary>
public class TenantSubscription
{
    public int Id { get; set; }
    
    /// <summary>
    /// Firma ID
    /// </summary>
    public int TenantId { get; set; }
    
    /// <summary>
    /// Abonelik planı ID
    /// </summary>
    public int SubscriptionPlanId { get; set; }
    
    /// <summary>
    /// Abonelik başlangıç tarihi
    /// </summary>
    public DateTime BaslangicTarihi { get; set; }
    
    /// <summary>
    /// Abonelik bitiş tarihi
    /// </summary>
    public DateTime BitisTarihi { get; set; }
    
    /// <summary>
    /// Ödeme tipi (Aylık/Yıllık)
    /// </summary>
    public OdemeTipi OdemeTipi { get; set; } = OdemeTipi.Aylik;
    
    /// <summary>
    /// Abonelik durumu
    /// </summary>
    public SubscriptionDurum Durum { get; set; } = SubscriptionDurum.Aktif;
    
    /// <summary>
    /// Oluşturma tarihi
    /// </summary>
    public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;
    
    // Navigation Properties
    public virtual Tenant? Tenant { get; set; }
    public virtual SubscriptionPlan? SubscriptionPlan { get; set; }
}
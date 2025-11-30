namespace ErpCrm.Core.Entities;

/// <summary>
/// Abonelik planı entity sınıfı
/// Farklı fiyat ve özellik paketlerini tanımlar
/// </summary>
public class SubscriptionPlan
{
    public int Id { get; set; }
    
    /// <summary>
    /// Benzersiz plan kodu (örn: STARTER, PROFESSIONAL, ENTERPRISE)
    /// </summary>
    public string PlanKodu { get; set; } = string.Empty;
    
    /// <summary>
    /// Plan adı
    /// </summary>
    public string PlanAdi { get; set; } = string.Empty;
    
    /// <summary>
    /// Plan açıklaması
    /// </summary>
    public string? Aciklama { get; set; }
    
    /// <summary>
    /// Aylık ücret
    /// </summary>
    public decimal AylikUcret { get; set; } = 0;
    
    /// <summary>
    /// Yıllık ücret (indirimli)
    /// </summary>
    public decimal YillikUcret { get; set; } = 0;
    
    /// <summary>
    /// Maksimum kullanıcı sayısı
    /// </summary>
    public int MaxKullanici { get; set; } = 1;
    
    /// <summary>
    /// Plan aktif mi?
    /// </summary>
    public bool Aktif { get; set; } = true;
    
    /// <summary>
    /// Oluşturma tarihi
    /// </summary>
    public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;
    
    // Navigation Properties
    public virtual ICollection<TenantSubscription> TenantSubscriptions { get; set; } = new List<TenantSubscription>();
    public virtual ICollection<PlanModule> PlanModules { get; set; } = new List<PlanModule>();
}
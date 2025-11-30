namespace ErpCrm.Core.Entities;

/// <summary>
/// Plan-Modül ilişki entity sınıfı
/// Hangi plana hangi modüllerin dahil olduğunu tanımlar
/// </summary>
public class PlanModule
{
    public int Id { get; set; }
    
    /// <summary>
    /// Abonelik planı ID
    /// </summary>
    public int SubscriptionPlanId { get; set; }
    
    /// <summary>
    /// Modül ID
    /// </summary>
    public int ModuleId { get; set; }
    
    /// <summary>
    /// Oluşturma tarihi
    /// </summary>
    public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;
    
    // Navigation Properties
    public virtual SubscriptionPlan? SubscriptionPlan { get; set; }
    public virtual Module? Module { get; set; }
}
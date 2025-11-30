namespace ErpCrm.Core.Entities;

/// <summary>
/// Firma-Modül ilişki entity sınıfı
/// Hangi firmanın hangi modüllere erişimi olduğunu tanımlar
/// </summary>
public class TenantModule
{
    public int Id { get; set; }
    
    /// <summary>
    /// Firma ID
    /// </summary>
    public int TenantId { get; set; }
    
    /// <summary>
    /// Modül ID
    /// </summary>
    public int ModuleId { get; set; }
    
    /// <summary>
    /// Modül aktif mi?
    /// </summary>
    public bool Aktif { get; set; } = true;
    
    /// <summary>
    /// Oluşturma tarihi
    /// </summary>
    public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;
    
    // Navigation Properties
    public virtual Tenant? Tenant { get; set; }
    public virtual Module? Module { get; set; }
}
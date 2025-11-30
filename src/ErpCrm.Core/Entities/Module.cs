namespace ErpCrm.Core.Entities;

/// <summary>
/// Sistem modülü entity sınıfı
/// ERP/CRM/POS modüllerini tanımlar
/// </summary>
public class Module
{
    public int Id { get; set; }
    
    /// <summary>
    /// Benzersiz modül kodu (örn: CARI, STOK, FATURA)
    /// </summary>
    public string ModulKodu { get; set; } = string.Empty;
    
    /// <summary>
    /// Modül adı
    /// </summary>
    public string ModulAdi { get; set; } = string.Empty;
    
    /// <summary>
    /// Modül açıklaması
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
    /// Modül kategorisi (ERP, CRM, POS, GENEL)
    /// </summary>
    public string? Kategori { get; set; }
    
    /// <summary>
    /// Modül aktif mi?
    /// </summary>
    public bool Aktif { get; set; } = true;
    
    /// <summary>
    /// Oluşturma tarihi
    /// </summary>
    public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;
    
    // Navigation Properties
    public virtual ICollection<PlanModule> PlanModules { get; set; } = new List<PlanModule>();
    public virtual ICollection<TenantModule> TenantModules { get; set; } = new List<TenantModule>();
}
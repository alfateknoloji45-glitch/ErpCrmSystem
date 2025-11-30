namespace ErpCrm.Core.Entities;

/// <summary>
/// Kullanıcı rollerini tanımlar
/// </summary>
public enum UserRol
{
    SuperAdmin = 0,
    TenantAdmin = 1,
    User = 2,
    Garson = 3,
    Kasiyer = 4
}

/// <summary>
/// Kullanıcı entity sınıfı
/// </summary>
public class User
{
    public int Id { get; set; }
    
    /// <summary>
    /// Kullanıcının bağlı olduğu firma ID'si
    /// </summary>
    public int TenantId { get; set; }
    
    /// <summary>
    /// Email adresi (benzersiz)
    /// </summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// SHA256 ile hashlenmiş şifre
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;
    
    /// <summary>
    /// Kullanıcı adı soyadı
    /// </summary>
    public string AdSoyad { get; set; } = string.Empty;
    
    /// <summary>
    /// Telefon numarası
    /// </summary>
    public string? Telefon { get; set; }
    
    /// <summary>
    /// Kullanıcı rolü
    /// </summary>
    public UserRol Rol { get; set; } = UserRol.User;
    
    /// <summary>
    /// Kullanıcı aktif mi?
    /// </summary>
    public bool Aktif { get; set; } = true;
    
    /// <summary>
    /// Son giriş tarihi
    /// </summary>
    public DateTime? SonGirisTarihi { get; set; }
    
    /// <summary>
    /// Kayıt oluşturma tarihi
    /// </summary>
    public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;
    
    /// <summary>
    /// Son güncelleme tarihi
    /// </summary>
    public DateTime? GuncellemeTarihi { get; set; }
    
    // Navigation Properties
    public virtual Tenant? Tenant { get; set; }
    public virtual ICollection<Fatura> OlusturulanFaturalar { get; set; } = new List<Fatura>();
    public virtual ICollection<Adisyon> Adisyonlar { get; set; } = new List<Adisyon>();
    public virtual ICollection<CrmMusteri> AtananMusteriler { get; set; } = new List<CrmMusteri>();
    public virtual ICollection<CrmAktivite> SorumluAktiviteler { get; set; } = new List<CrmAktivite>();
}
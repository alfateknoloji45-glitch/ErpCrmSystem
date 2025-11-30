namespace ErpCrm.Application.DTOs;

/// <summary>
/// Login yanıtı DTO sınıfı
/// </summary>
public class LoginResponse
{
    /// <summary>
    /// Kullanıcı ID
    /// </summary>
    public int UserId { get; set; }
    
    /// <summary>
    /// Kullanıcı adı soyadı
    /// </summary>
    public string AdSoyad { get; set; } = string.Empty;
    
    /// <summary>
    /// Kullanıcı email adresi
    /// </summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// Kullanıcı rolü
    /// </summary>
    public string Rol { get; set; } = string.Empty;
    
    /// <summary>
    /// Firma ID
    /// </summary>
    public int TenantId { get; set; }
    
    /// <summary>
    /// Firma adı
    /// </summary>
    public string FirmaAdi { get; set; } = string.Empty;
    
    /// <summary>
    /// JWT Token
    /// </summary>
    public string Token { get; set; } = string.Empty;
    
    /// <summary>
    /// Kullanıcının erişebildiği aktif modüller
    /// </summary>
    public List<string> AktifModuller { get; set; } = new();
    
    /// <summary>
    /// Demo hesap mı?
    /// </summary>
    public bool DemoMu { get; set; }
    
    /// <summary>
    /// Demo bitiş tarihi
    /// </summary>
    public DateTime? DemoBitisTarihi { get; set; }
}
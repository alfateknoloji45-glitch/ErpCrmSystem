namespace ErpCrm.Desktop.Models;

/// <summary>
/// Login isteği model sınıfı
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// Kullanıcı email adresi
    /// </summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// Kullanıcı şifresi
    /// </summary>
    public string Password { get; set; } = string.Empty;
}
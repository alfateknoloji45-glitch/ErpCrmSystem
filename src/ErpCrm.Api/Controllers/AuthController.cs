using Microsoft.AspNetCore.Mvc;
using ErpCrm.Application.DTOs;
using ErpCrm.Application.Services;

namespace ErpCrm.Api.Controllers;

/// <summary>
/// Kimlik doğrulama controller'ı
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Kullanıcı girişi yapar
    /// </summary>
    /// <param name="request">Login isteği (Email, Password)</param>
    /// <returns>Login yanıtı veya hata</returns>
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        try
        {
            // Giriş bilgilerini doğrula
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return BadRequest(new { message = "Email adresi gereklidir." });
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { message = "Şifre gereklidir." });
            }

            // Login işlemini gerçekleştir
            var result = await _authService.LoginAsync(request);

            if (result == null)
            {
                return Unauthorized(new { message = "Email veya şifre hatalı." });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Sunucu hatası oluştu.", error = ex.Message });
        }
    }

    /// <summary>
    /// Token'ı doğrular
    /// </summary>
    /// <param name="token">JWT Token</param>
    /// <returns>Doğrulama sonucu</returns>
    [HttpGet("validate")]
    public ActionResult ValidateToken([FromQuery] string token)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return BadRequest(new { valid = false, message = "Token gereklidir." });
            }

            var userId = _authService.ValidateToken(token);

            if (userId == null)
            {
                return Ok(new { valid = false, message = "Geçersiz veya süresi dolmuş token." });
            }

            return Ok(new { valid = true, userId = userId });
        }
        catch (Exception)
        {
            return Ok(new { valid = false, message = "Token doğrulanamadı." });
        }
    }
}
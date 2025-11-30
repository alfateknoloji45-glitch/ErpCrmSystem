using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using ErpCrm.Application.DTOs;
using ErpCrm.Core.Entities;
using ErpCrm.Infrastructure.Data;

namespace ErpCrm.Application.Services;

/// <summary>
/// Kimlik doğrulama servisi
/// </summary>
public class AuthService
{
    private readonly AppDbContext _context;

    public AuthService(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Kullanıcı girişi yapar
    /// </summary>
    /// <param name="request">Login isteği</param>
    /// <returns>Login yanıtı veya null (başarısız)</returns>
    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        try
        {
            // Email kontrolü
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return null;
            }

            // Şifreyi hashle
            var passwordHash = HashPassword(request.Password);

            // Kullanıcıyı bul
            var user = await _context.Users
                .Include(u => u.Tenant)
                .FirstOrDefaultAsync(u => 
                    u.Email.ToLower() == request.Email.ToLower() && 
                    u.PasswordHash == passwordHash &&
                    u.Aktif);

            if (user == null || user.Tenant == null)
            {
                return null;
            }

            // Tenant durumunu kontrol et
            if (user.Tenant.Durum == TenantDurum.Pasif || user.Tenant.Durum == TenantDurum.Askida)
            {
                return null;
            }

            // Demo süresi kontrolü
            if (user.Tenant.DemoMu && user.Tenant.DemoBitisTarihi.HasValue)
            {
                if (user.Tenant.DemoBitisTarihi.Value < DateTime.Now)
                {
                    return null;
                }
            }

            // Aktif modülleri çek
            var aktifModuller = await _context.TenantModules
                .Include(tm => tm.Module)
                .Where(tm => tm.TenantId == user.TenantId && tm.Aktif && tm.Module != null && tm.Module.Aktif)
                .Select(tm => tm.Module!.ModulKodu)
                .ToListAsync();

            // Son giriş tarihini güncelle
            user.SonGirisTarihi = DateTime.Now;
            await _context.SaveChangesAsync();

            // Token oluştur
            var token = GenerateToken(user);

            return new LoginResponse
            {
                UserId = user.Id,
                AdSoyad = user.AdSoyad,
                Email = user.Email,
                Rol = user.Rol.ToString(),
                TenantId = user.TenantId,
                FirmaAdi = user.Tenant.FirmaAdi,
                Token = token,
                AktifModuller = aktifModuller,
                DemoMu = user.Tenant.DemoMu,
                DemoBitisTarihi = user.Tenant.DemoBitisTarihi
            };
        }
        catch (Exception)
        {
            // Hata durumunda null dön
            return null;
        }
    }

    /// <summary>
    /// Şifreyi SHA256 ile hashler
    /// </summary>
    /// <param name="password">Düz metin şifre</param>
    /// <returns>Base64 kodlanmış hash</returns>
    public static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    /// <summary>
    /// Basit JWT token oluşturur
    /// NOT: Production'da gerçek JWT kütüphanesi kullanılmalı
    /// </summary>
    /// <param name="user">Kullanıcı</param>
    /// <returns>Token string</returns>
    private string GenerateToken(User user)
    {
        // Basit token oluşturma (production'da JWT kullanılmalı)
        var tokenData = $"{user.Id}|{user.TenantId}|{user.Rol}|{DateTime.UtcNow.AddHours(24).Ticks}";
        var bytes = Encoding.UTF8.GetBytes(tokenData);
        return Convert.ToBase64String(bytes);
    }

    /// <summary>
    /// Token'ı doğrular
    /// </summary>
    /// <param name="token">Token string</param>
    /// <returns>Kullanıcı ID veya null</returns>
    public int? ValidateToken(string token)
    {
        try
        {
            var bytes = Convert.FromBase64String(token);
            var tokenData = Encoding.UTF8.GetString(bytes);
            var parts = tokenData.Split('|');

            if (parts.Length != 4)
            {
                return null;
            }

            var expireTicks = long.Parse(parts[3]);
            if (new DateTime(expireTicks) < DateTime.UtcNow)
            {
                return null;
            }

            return int.Parse(parts[0]);
        }
        catch
        {
            return null;
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ErpCrm.Infrastructure.Data;

namespace ErpCrm.Api.Controllers;

/// <summary>
/// Dashboard istatistikleri için DTO
/// </summary>
public class DashboardStatsDto
{
    public int ToplamMusteri { get; set; }
    public int ToplamUrun { get; set; }
    public int ToplamFatura { get; set; }
}

/// <summary>
/// Dashboard controller'ı - Ana sayfa istatistiklerini döndürür
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly AppDbContext _context;

    public DashboardController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Tenant ID'yi header'dan alır
    /// </summary>
    private int GetTenantId()
    {
        if (Request.Headers.TryGetValue("X-Tenant-Id", out var tenantIdHeader))
        {
            if (int.TryParse(tenantIdHeader, out int tenantId))
            {
                return tenantId;
            }
        }
        return 0;
    }

    /// <summary>
    /// Dashboard istatistiklerini getirir
    /// </summary>
    /// <returns>Toplam müşteri, ürün ve fatura sayıları</returns>
    [HttpGet("stats")]
    public async Task<ActionResult<DashboardStatsDto>> GetStats()
    {
        try
        {
            var tenantId = GetTenantId();
            if (tenantId == 0)
            {
                return BadRequest(new { message = "X-Tenant-Id header'ı gereklidir." });
            }

            var toplamMusteri = await _context.Cariler
                .Where(c => c.TenantId == tenantId && c.Aktif)
                .CountAsync();

            var toplamUrun = await _context.StokKartlari
                .Where(s => s.TenantId == tenantId && s.Aktif)
                .CountAsync();

            var toplamFatura = await _context.Faturalar
                .Where(f => f.TenantId == tenantId)
                .CountAsync();

            return Ok(new DashboardStatsDto
            {
                ToplamMusteri = toplamMusteri,
                ToplamUrun = toplamUrun,
                ToplamFatura = toplamFatura
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Sunucu hatası.", error = ex.Message });
        }
    }
}

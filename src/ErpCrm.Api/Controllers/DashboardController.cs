using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ErpCrm.Infrastructure.Data;

namespace ErpCrm.Api.Controllers;

/// <summary>
/// Dashboard istatistikleri controller'ı
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
    /// <returns>Dashboard istatistikleri</returns>
    [HttpGet("stats")]
    public async Task<ActionResult<object>> GetStats()
    {
        try
        {
            var tenantId = GetTenantId();
            if (tenantId == 0)
            {
                return BadRequest(new { message = "X-Tenant-Id header'ı gereklidir." });
            }

            var customerCount = await _context.Cariler
                .Where(c => c.TenantId == tenantId && c.Aktif)
                .CountAsync();

            var productCount = await _context.StokKartlari
                .Where(s => s.TenantId == tenantId && s.Aktif)
                .CountAsync();

            var invoiceCount = await _context.Faturalar
                .Where(f => f.TenantId == tenantId)
                .CountAsync();

            var totalRevenue = await _context.Faturalar
                .Where(f => f.TenantId == tenantId && f.FaturaTipi == ErpCrm.Core.Entities.FaturaTipi.Satis)
                .SumAsync(f => f.GenelToplam);

            var lowStockCount = await _context.StokKartlari
                .Where(s => s.TenantId == tenantId && s.StokMiktari <= s.MinStokMiktari && s.Aktif)
                .CountAsync();

            var recentInvoices = await _context.Faturalar
                .Include(f => f.Cari)
                .Where(f => f.TenantId == tenantId)
                .OrderByDescending(f => f.FaturaTarihi)
                .Take(5)
                .Select(f => new
                {
                    f.Id,
                    f.FaturaNo,
                    f.FaturaTarihi,
                    FaturaTipi = f.FaturaTipi.ToString(),
                    CariAdi = f.Cari != null ? f.Cari.CariAdi : "",
                    f.GenelToplam,
                    Durum = f.Durum.ToString()
                })
                .ToListAsync();

            return Ok(new
            {
                CustomerCount = customerCount,
                ProductCount = productCount,
                InvoiceCount = invoiceCount,
                TotalRevenue = totalRevenue,
                LowStockCount = lowStockCount,
                RecentInvoices = recentInvoices
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Sunucu hatası.", error = ex.Message });
        }
    }
}

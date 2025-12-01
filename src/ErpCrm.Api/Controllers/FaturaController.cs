using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ErpCrm.Core.Entities;
using ErpCrm.Infrastructure.Data;

namespace ErpCrm.Api.Controllers;

/// <summary>
/// Fatura yönetimi controller'ı
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class FaturaController : ControllerBase
{
    private readonly AppDbContext _context;

    public FaturaController(AppDbContext context)
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
    /// Tüm faturaları listeler
    /// </summary>
    /// <returns>Fatura listesi</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetAll()
    {
        try
        {
            var tenantId = GetTenantId();
            if (tenantId == 0)
            {
                return BadRequest(new { message = "X-Tenant-Id header'ı gereklidir." });
            }

            var faturalar = await _context.Faturalar
                .Include(f => f.Cari)
                .Where(f => f.TenantId == tenantId)
                .OrderByDescending(f => f.FaturaTarihi)
                .Select(f => new
                {
                    f.Id,
                    f.FaturaNo,
                    f.FaturaTarihi,
                    FaturaTipi = f.FaturaTipi.ToString(),
                    f.CariId,
                    CariAdi = f.Cari != null ? f.Cari.CariAdi : "",
                    f.AraToplam,
                    f.KdvToplam,
                    f.IndirimToplam,
                    f.GenelToplam,
                    Durum = f.Durum.ToString(),
                    f.Aciklama,
                    f.OlusturmaTarihi
                })
                .ToListAsync();

            return Ok(faturalar);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Sunucu hatası.", error = ex.Message });
        }
    }

    /// <summary>
    /// ID'ye göre fatura getirir
    /// </summary>
    /// <param name="id">Fatura ID</param>
    /// <returns>Fatura bilgisi</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<Fatura>> GetById(int id)
    {
        try
        {
            var tenantId = GetTenantId();
            if (tenantId == 0)
            {
                return BadRequest(new { message = "X-Tenant-Id header'ı gereklidir." });
            }

            var fatura = await _context.Faturalar
                .Include(f => f.Cari)
                .Include(f => f.FaturaSatirlari)
                .FirstOrDefaultAsync(f => f.Id == id && f.TenantId == tenantId);

            if (fatura == null)
            {
                return NotFound(new { message = "Fatura bulunamadı." });
            }

            return Ok(fatura);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Sunucu hatası.", error = ex.Message });
        }
    }

    /// <summary>
    /// Son faturaları getirir (Dashboard için)
    /// </summary>
    /// <param name="count">Fatura sayısı</param>
    /// <returns>Son faturalar</returns>
    [HttpGet("recent/{count}")]
    public async Task<ActionResult<IEnumerable<object>>> GetRecent(int count = 5)
    {
        try
        {
            var tenantId = GetTenantId();
            if (tenantId == 0)
            {
                return BadRequest(new { message = "X-Tenant-Id header'ı gereklidir." });
            }

            var faturalar = await _context.Faturalar
                .Include(f => f.Cari)
                .Where(f => f.TenantId == tenantId)
                .OrderByDescending(f => f.FaturaTarihi)
                .Take(count)
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

            return Ok(faturalar);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Sunucu hatası.", error = ex.Message });
        }
    }
}

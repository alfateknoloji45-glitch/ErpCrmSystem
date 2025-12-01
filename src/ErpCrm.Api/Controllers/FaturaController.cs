using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ErpCrm.Core.Entities;
using ErpCrm.Infrastructure.Data;

namespace ErpCrm.Api.Controllers;

/// <summary>
/// Fatura listesi için DTO
/// </summary>
public class FaturaListDto
{
    public int Id { get; set; }
    public string FaturaNo { get; set; } = string.Empty;
    public DateTime FaturaTarihi { get; set; }
    public string CariAdi { get; set; } = string.Empty;
    public decimal GenelToplam { get; set; }
    public string Durum { get; set; } = string.Empty;
}

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
    public async Task<ActionResult<IEnumerable<FaturaListDto>>> GetAll()
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
                .Select(f => new FaturaListDto
                {
                    Id = f.Id,
                    FaturaNo = f.FaturaNo,
                    FaturaTarihi = f.FaturaTarihi,
                    CariAdi = f.Cari != null ? f.Cari.CariAdi : "",
                    GenelToplam = f.GenelToplam,
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
                .ThenInclude(fs => fs.Stok)
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
    /// Fatura arama yapar
    /// </summary>
    /// <param name="q">Arama metni</param>
    /// <returns>Arama sonuçları</returns>
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<FaturaListDto>>> Search([FromQuery] string q)
    {
        try
        {
            var tenantId = GetTenantId();
            if (tenantId == 0)
            {
                return BadRequest(new { message = "X-Tenant-Id header'ı gereklidir." });
            }

            if (string.IsNullOrWhiteSpace(q))
            {
                return Ok(new List<FaturaListDto>());
            }

            var searchTerm = q.ToLower();
            var faturalar = await _context.Faturalar
                .Include(f => f.Cari)
                .Where(f => f.TenantId == tenantId &&
                    (f.FaturaNo.ToLower().Contains(searchTerm) ||
                     (f.Cari != null && f.Cari.CariAdi.ToLower().Contains(searchTerm))))
                .OrderByDescending(f => f.FaturaTarihi)
                .Take(50)
                .Select(f => new FaturaListDto
                {
                    Id = f.Id,
                    FaturaNo = f.FaturaNo,
                    FaturaTarihi = f.FaturaTarihi,
                    CariAdi = f.Cari != null ? f.Cari.CariAdi : "",
                    GenelToplam = f.GenelToplam,
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

    /// <summary>
    /// Fatura siler
    /// </summary>
    /// <param name="id">Fatura ID</param>
    /// <returns>Silme sonucu</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            var tenantId = GetTenantId();
            if (tenantId == 0)
            {
                return BadRequest(new { message = "X-Tenant-Id header'ı gereklidir." });
            }

            var fatura = await _context.Faturalar
                .FirstOrDefaultAsync(f => f.Id == id && f.TenantId == tenantId);

            if (fatura == null)
            {
                return NotFound(new { message = "Fatura bulunamadı." });
            }

            // Onaylanmış faturalar silinemez
            if (fatura.Durum == FaturaDurum.Onaylandi)
            {
                return BadRequest(new { message = "Onaylanmış faturalar silinemez." });
            }

            _context.Faturalar.Remove(fatura);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Fatura başarıyla silindi." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Sunucu hatası.", error = ex.Message });
        }
    }
}

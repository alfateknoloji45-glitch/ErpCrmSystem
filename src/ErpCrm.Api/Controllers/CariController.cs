using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ErpCrm.Core.Entities;
using ErpCrm.Infrastructure.Data;

namespace ErpCrm.Api.Controllers;

/// <summary>
/// Cari hesap yönetimi controller'ı
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CariController : ControllerBase
{
    private readonly AppDbContext _context;

    public CariController(AppDbContext context)
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
    /// Tüm carileri listeler
    /// </summary>
    /// <returns>Cari listesi</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cari>>> GetAll()
    {
        try
        {
            var tenantId = GetTenantId();
            if (tenantId == 0)
            {
                return BadRequest(new { message = "X-Tenant-Id header'ı gereklidir." });
            }

            var cariler = await _context.Cariler
                .Where(c => c.TenantId == tenantId)
                .OrderBy(c => c.CariAdi)
                .ToListAsync();

            return Ok(cariler);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Sunucu hatası.", error = ex.Message });
        }
    }

    /// <summary>
    /// ID'ye göre cari getirir
    /// </summary>
    /// <param name="id">Cari ID</param>
    /// <returns>Cari bilgisi</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<Cari>> GetById(int id)
    {
        try
        {
            var tenantId = GetTenantId();
            if (tenantId == 0)
            {
                return BadRequest(new { message = "X-Tenant-Id header'ı gereklidir." });
            }

            var cari = await _context.Cariler
                .FirstOrDefaultAsync(c => c.Id == id && c.TenantId == tenantId);

            if (cari == null)
            {
                return NotFound(new { message = "Cari bulunamadı." });
            }

            return Ok(cari);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Sunucu hatası.", error = ex.Message });
        }
    }

    /// <summary>
    /// Yeni cari oluşturur
    /// </summary>
    /// <param name="cari">Cari bilgisi</param>
    /// <returns>Oluşturulan cari</returns>
    [HttpPost]
    public async Task<ActionResult<Cari>> Create([FromBody] Cari cari)
    {
        try
        {
            var tenantId = GetTenantId();
            if (tenantId == 0)
            {
                return BadRequest(new { message = "X-Tenant-Id header'ı gereklidir." });
            }

            // Zorunlu alan kontrolü
            if (string.IsNullOrWhiteSpace(cari.CariKodu))
            {
                return BadRequest(new { message = "Cari kodu gereklidir." });
            }

            if (string.IsNullOrWhiteSpace(cari.CariAdi))
            {
                return BadRequest(new { message = "Cari adı gereklidir." });
            }

            // Kod benzersizlik kontrolü
            var existingCari = await _context.Cariler
                .FirstOrDefaultAsync(c => c.TenantId == tenantId && c.CariKodu == cari.CariKodu);

            if (existingCari != null)
            {
                return BadRequest(new { message = "Bu cari kodu zaten kullanılıyor." });
            }

            // Tenant ID'yi ata
            cari.TenantId = tenantId;
            cari.OlusturmaTarihi = DateTime.Now;

            _context.Cariler.Add(cari);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = cari.Id }, cari);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Sunucu hatası.", error = ex.Message });
        }
    }

    /// <summary>
    /// Cari bilgilerini günceller
    /// </summary>
    /// <param name="id">Cari ID</param>
    /// <param name="cari">Güncel cari bilgisi</param>
    /// <returns>Güncellenen cari</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<Cari>> Update(int id, [FromBody] Cari cari)
    {
        try
        {
            var tenantId = GetTenantId();
            if (tenantId == 0)
            {
                return BadRequest(new { message = "X-Tenant-Id header'ı gereklidir." });
            }

            var existingCari = await _context.Cariler
                .FirstOrDefaultAsync(c => c.Id == id && c.TenantId == tenantId);

            if (existingCari == null)
            {
                return NotFound(new { message = "Cari bulunamadı." });
            }

            // Kod değişikliği varsa benzersizlik kontrolü
            if (existingCari.CariKodu != cari.CariKodu)
            {
                var duplicateCari = await _context.Cariler
                    .FirstOrDefaultAsync(c => c.TenantId == tenantId && c.CariKodu == cari.CariKodu);

                if (duplicateCari != null)
                {
                    return BadRequest(new { message = "Bu cari kodu zaten kullanılıyor." });
                }
            }

            // Güncellenebilir alanları güncelle
            existingCari.CariKodu = cari.CariKodu;
            existingCari.CariAdi = cari.CariAdi;
            existingCari.CariTip = cari.CariTip;
            existingCari.VergiDairesi = cari.VergiDairesi;
            existingCari.VergiNo = cari.VergiNo;
            existingCari.Telefon = cari.Telefon;
            existingCari.Email = cari.Email;
            existingCari.Adres = cari.Adres;
            existingCari.Il = cari.Il;
            existingCari.Ilce = cari.Ilce;
            existingCari.AlacakLimiti = cari.AlacakLimiti;
            existingCari.Aktif = cari.Aktif;
            existingCari.GuncellemeTarihi = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(existingCari);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Sunucu hatası.", error = ex.Message });
        }
    }

    /// <summary>
    /// Cari siler
    /// </summary>
    /// <param name="id">Cari ID</param>
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

            var cari = await _context.Cariler
                .FirstOrDefaultAsync(c => c.Id == id && c.TenantId == tenantId);

            if (cari == null)
            {
                return NotFound(new { message = "Cari bulunamadı." });
            }

            // İlişkili fatura kontrolü
            var hasFatura = await _context.Faturalar.AnyAsync(f => f.CariId == id);
            if (hasFatura)
            {
                return BadRequest(new { message = "Bu cariye ait faturalar bulunduğu için silinemez." });
            }

            _context.Cariler.Remove(cari);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Cari başarıyla silindi." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Sunucu hatası.", error = ex.Message });
        }
    }

    /// <summary>
    /// Cari tipine göre arama yapar
    /// </summary>
    /// <param name="tip">Cari tipi (0: Müşteri, 1: Tedarikçi, 2: Her İkisi)</param>
    /// <returns>Filtrelenmiş cari listesi</returns>
    [HttpGet("tip/{tip}")]
    public async Task<ActionResult<IEnumerable<Cari>>> GetByTip(int tip)
    {
        try
        {
            var tenantId = GetTenantId();
            if (tenantId == 0)
            {
                return BadRequest(new { message = "X-Tenant-Id header'ı gereklidir." });
            }

            var cariler = await _context.Cariler
                .Where(c => c.TenantId == tenantId && (int)c.CariTip == tip)
                .OrderBy(c => c.CariAdi)
                .ToListAsync();

            return Ok(cariler);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Sunucu hatası.", error = ex.Message });
        }
    }

    /// <summary>
    /// Cari arama yapar
    /// </summary>
    /// <param name="q">Arama metni</param>
    /// <returns>Arama sonuçları</returns>
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<Cari>>> Search([FromQuery] string q)
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
                return Ok(new List<Cari>());
            }

            var searchTerm = q.ToLower();
            var cariler = await _context.Cariler
                .Where(c => c.TenantId == tenantId &&
                    (c.CariKodu.ToLower().Contains(searchTerm) ||
                     c.CariAdi.ToLower().Contains(searchTerm) ||
                     (c.Telefon != null && c.Telefon.Contains(searchTerm)) ||
                     (c.Email != null && c.Email.ToLower().Contains(searchTerm))))
                .OrderBy(c => c.CariAdi)
                .Take(50)
                .ToListAsync();

            return Ok(cariler);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Sunucu hatası.", error = ex.Message });
        }
    }
}
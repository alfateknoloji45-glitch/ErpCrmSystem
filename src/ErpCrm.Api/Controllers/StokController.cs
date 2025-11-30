using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ErpCrm.Core.Entities;
using ErpCrm.Infrastructure.Data;

namespace ErpCrm.Api.Controllers;

/// <summary>
/// Stok kartı yönetimi controller'ı
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class StokController : ControllerBase
{
    private readonly AppDbContext _context;

    public StokController(AppDbContext context)
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
    /// Tüm stok kartlarını listeler
    /// </summary>
    /// <returns>Stok kartı listesi</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<StokKarti>>> GetAll()
    {
        try
        {
            var tenantId = GetTenantId();
            if (tenantId == 0)
            {
                return BadRequest(new { message = "X-Tenant-Id header'ı gereklidir." });
            }

            var stoklar = await _context.StokKartlari
                .Where(s => s.TenantId == tenantId)
                .OrderBy(s => s.StokAdi)
                .ToListAsync();

            return Ok(stoklar);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Sunucu hatası.", error = ex.Message });
        }
    }

    /// <summary>
    /// ID'ye göre stok kartı getirir
    /// </summary>
    /// <param name="id">Stok ID</param>
    /// <returns>Stok kartı bilgisi</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<StokKarti>> GetById(int id)
    {
        try
        {
            var tenantId = GetTenantId();
            if (tenantId == 0)
            {
                return BadRequest(new { message = "X-Tenant-Id header'ı gereklidir." });
            }

            var stok = await _context.StokKartlari
                .FirstOrDefaultAsync(s => s.Id == id && s.TenantId == tenantId);

            if (stok == null)
            {
                return NotFound(new { message = "Stok kartı bulunamadı." });
            }

            return Ok(stok);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Sunucu hatası.", error = ex.Message });
        }
    }

    /// <summary>
    /// Yeni stok kartı oluşturur
    /// </summary>
    /// <param name="stok">Stok kartı bilgisi</param>
    /// <returns>Oluşturulan stok kartı</returns>
    [HttpPost]
    public async Task<ActionResult<StokKarti>> Create([FromBody] StokKarti stok)
    {
        try
        {
            var tenantId = GetTenantId();
            if (tenantId == 0)
            {
                return BadRequest(new { message = "X-Tenant-Id header'ı gereklidir." });
            }

            // Zorunlu alan kontrolü
            if (string.IsNullOrWhiteSpace(stok.StokKodu))
            {
                return BadRequest(new { message = "Stok kodu gereklidir." });
            }

            if (string.IsNullOrWhiteSpace(stok.StokAdi))
            {
                return BadRequest(new { message = "Stok adı gereklidir." });
            }

            // Kod benzersizlik kontrolü
            var existingStok = await _context.StokKartlari
                .FirstOrDefaultAsync(s => s.TenantId == tenantId && s.StokKodu == stok.StokKodu);

            if (existingStok != null)
            {
                return BadRequest(new { message = "Bu stok kodu zaten kullanılıyor." });
            }

            // Barkod benzersizlik kontrolü (eğer varsa)
            if (!string.IsNullOrWhiteSpace(stok.Barkod))
            {
                var existingBarkod = await _context.StokKartlari
                    .FirstOrDefaultAsync(s => s.TenantId == tenantId && s.Barkod == stok.Barkod);

                if (existingBarkod != null)
                {
                    return BadRequest(new { message = "Bu barkod zaten kullanılıyor." });
                }
            }

            // Tenant ID'yi ata
            stok.TenantId = tenantId;
            stok.OlusturmaTarihi = DateTime.Now;

            _context.StokKartlari.Add(stok);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = stok.Id }, stok);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Sunucu hatası.", error = ex.Message });
        }
    }

    /// <summary>
    /// Stok kartı bilgilerini günceller
    /// </summary>
    /// <param name="id">Stok ID</param>
    /// <param name="stok">Güncel stok kartı bilgisi</param>
    /// <returns>Güncellenen stok kartı</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<StokKarti>> Update(int id, [FromBody] StokKarti stok)
    {
        try
        {
            var tenantId = GetTenantId();
            if (tenantId == 0)
            {
                return BadRequest(new { message = "X-Tenant-Id header'ı gereklidir." });
            }

            var existingStok = await _context.StokKartlari
                .FirstOrDefaultAsync(s => s.Id == id && s.TenantId == tenantId);

            if (existingStok == null)
            {
                return NotFound(new { message = "Stok kartı bulunamadı." });
            }

            // Kod değişikliği varsa benzersizlik kontrolü
            if (existingStok.StokKodu != stok.StokKodu)
            {
                var duplicateStok = await _context.StokKartlari
                    .FirstOrDefaultAsync(s => s.TenantId == tenantId && s.StokKodu == stok.StokKodu);

                if (duplicateStok != null)
                {
                    return BadRequest(new { message = "Bu stok kodu zaten kullanılıyor." });
                }
            }

            // Barkod değişikliği varsa benzersizlik kontrolü
            if (!string.IsNullOrWhiteSpace(stok.Barkod) && existingStok.Barkod != stok.Barkod)
            {
                var duplicateBarkod = await _context.StokKartlari
                    .FirstOrDefaultAsync(s => s.TenantId == tenantId && s.Barkod == stok.Barkod);

                if (duplicateBarkod != null)
                {
                    return BadRequest(new { message = "Bu barkod zaten kullanılıyor." });
                }
            }

            // Güncellenebilir alanları güncelle
            existingStok.StokKodu = stok.StokKodu;
            existingStok.StokAdi = stok.StokAdi;
            existingStok.Barkod = stok.Barkod;
            existingStok.Birim = stok.Birim;
            existingStok.Kategori = stok.Kategori;
            existingStok.AltKategori = stok.AltKategori;
            existingStok.AlisFiyati = stok.AlisFiyati;
            existingStok.SatisFiyati = stok.SatisFiyati;
            existingStok.KdvOrani = stok.KdvOrani;
            existingStok.MinStokMiktari = stok.MinStokMiktari;
            existingStok.Aciklama = stok.Aciklama;
            existingStok.Aktif = stok.Aktif;
            existingStok.GuncellemeTarihi = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(existingStok);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Sunucu hatası.", error = ex.Message });
        }
    }

    /// <summary>
    /// Stok kartı siler
    /// </summary>
    /// <param name="id">Stok ID</param>
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

            var stok = await _context.StokKartlari
                .FirstOrDefaultAsync(s => s.Id == id && s.TenantId == tenantId);

            if (stok == null)
            {
                return NotFound(new { message = "Stok kartı bulunamadı." });
            }

            // İlişkili satır kontrolü
            var hasFaturaSatiri = await _context.FaturaSatirlari.AnyAsync(fs => fs.StokId == id);
            var hasAdisyonSatiri = await _context.AdisyonSatirlari.AnyAsync(ads => ads.StokId == id);

            if (hasFaturaSatiri || hasAdisyonSatiri)
            {
                return BadRequest(new { message = "Bu stok kartına ait işlemler bulunduğu için silinemez." });
            }

            _context.StokKartlari.Remove(stok);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Stok kartı başarıyla silindi." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Sunucu hatası.", error = ex.Message });
        }
    }

    /// <summary>
    /// Kategoriye göre stok kartlarını listeler
    /// </summary>
    /// <param name="kategori">Kategori adı</param>
    /// <returns>Filtrelenmiş stok kartı listesi</returns>
    [HttpGet("kategori/{kategori}")]
    public async Task<ActionResult<IEnumerable<StokKarti>>> GetByKategori(string kategori)
    {
        try
        {
            var tenantId = GetTenantId();
            if (tenantId == 0)
            {
                return BadRequest(new { message = "X-Tenant-Id header'ı gereklidir." });
            }

            var stoklar = await _context.StokKartlari
                .Where(s => s.TenantId == tenantId && s.Kategori == kategori)
                .OrderBy(s => s.StokAdi)
                .ToListAsync();

            return Ok(stoklar);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Sunucu hatası.", error = ex.Message });
        }
    }

    /// <summary>
    /// Barkoda göre stok kartı getirir
    /// </summary>
    /// <param name="barkod">Barkod numarası</param>
    /// <returns>Stok kartı</returns>
    [HttpGet("barkod/{barkod}")]
    public async Task<ActionResult<StokKarti>> GetByBarkod(string barkod)
    {
        try
        {
            var tenantId = GetTenantId();
            if (tenantId == 0)
            {
                return BadRequest(new { message = "X-Tenant-Id header'ı gereklidir." });
            }

            var stok = await _context.StokKartlari
                .FirstOrDefaultAsync(s => s.TenantId == tenantId && s.Barkod == barkod);

            if (stok == null)
            {
                return NotFound(new { message = "Stok kartı bulunamadı." });
            }

            return Ok(stok);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Sunucu hatası.", error = ex.Message });
        }
    }

    /// <summary>
    /// Stok kartı arama yapar
    /// </summary>
    /// <param name="q">Arama metni</param>
    /// <returns>Arama sonuçları</returns>
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<StokKarti>>> Search([FromQuery] string q)
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
                return Ok(new List<StokKarti>());
            }

            var searchTerm = q.ToLower();
            var stoklar = await _context.StokKartlari
                .Where(s => s.TenantId == tenantId &&
                    (s.StokKodu.ToLower().Contains(searchTerm) ||
                     s.StokAdi.ToLower().Contains(searchTerm) ||
                     (s.Barkod != null && s.Barkod.Contains(searchTerm)) ||
                     (s.Kategori != null && s.Kategori.ToLower().Contains(searchTerm))))
                .OrderBy(s => s.StokAdi)
                .Take(50)
                .ToListAsync();

            return Ok(stoklar);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Sunucu hatası.", error = ex.Message });
        }
    }

    /// <summary>
    /// Düşük stok uyarısı için stok kartlarını listeler
    /// </summary>
    /// <returns>Minimum stok altındaki stok kartları</returns>
    [HttpGet("lowstock")]
    public async Task<ActionResult<IEnumerable<StokKarti>>> GetLowStock()
    {
        try
        {
            var tenantId = GetTenantId();
            if (tenantId == 0)
            {
                return BadRequest(new { message = "X-Tenant-Id header'ı gereklidir." });
            }

            var stoklar = await _context.StokKartlari
                .Where(s => s.TenantId == tenantId && s.StokMiktari <= s.MinStokMiktari && s.Aktif)
                .OrderBy(s => s.StokMiktari)
                .ToListAsync();

            return Ok(stoklar);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Sunucu hatası.", error = ex.Message });
        }
    }
}
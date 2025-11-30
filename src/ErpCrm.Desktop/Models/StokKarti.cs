namespace ErpCrm.Desktop.Models;

/// <summary>
/// Stok kartı model sınıfı
/// </summary>
public class StokKarti
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public string StokKodu { get; set; } = string.Empty;
    public string StokAdi { get; set; } = string.Empty;
    public string? Barkod { get; set; }
    public string Birim { get; set; } = "Adet";
    public string? Kategori { get; set; }
    public string? AltKategori { get; set; }
    public decimal AlisFiyati { get; set; }
    public decimal SatisFiyati { get; set; }
    public int KdvOrani { get; set; } = 18;
    public decimal StokMiktari { get; set; }
    public decimal MinStokMiktari { get; set; }
    public string? Aciklama { get; set; }
    public bool Aktif { get; set; } = true;
    public DateTime OlusturmaTarihi { get; set; }
    public DateTime? GuncellemeTarihi { get; set; }
}
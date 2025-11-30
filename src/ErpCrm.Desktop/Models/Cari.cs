namespace ErpCrm.Desktop.Models;

/// <summary>
/// Cari tipi enum
/// </summary>
public enum CariTip
{
    Musteri = 0,
    Tedarikci = 1,
    HerIkisi = 2
}

/// <summary>
/// Cari model sınıfı
/// </summary>
public class Cari
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public string CariKodu { get; set; } = string.Empty;
    public string CariAdi { get; set; } = string.Empty;
    public CariTip CariTip { get; set; }
    public string? VergiDairesi { get; set; }
    public string? VergiNo { get; set; }
    public string? Telefon { get; set; }
    public string? Email { get; set; }
    public string? Adres { get; set; }
    public string? Il { get; set; }
    public string? Ilce { get; set; }
    public decimal Bakiye { get; set; }
    public decimal AlacakLimiti { get; set; }
    public bool Aktif { get; set; } = true;
    public DateTime OlusturmaTarihi { get; set; }
    public DateTime? GuncellemeTarihi { get; set; }

    /// <summary>
    /// Cari tipinin Türkçe karşılığı
    /// </summary>
    public string CariTipText => CariTip switch
    {
        CariTip.Musteri => "Müşteri",
        CariTip.Tedarikci => "Tedarikçi",
        CariTip.HerIkisi => "Her İkisi",
        _ => "Bilinmiyor"
    };
}
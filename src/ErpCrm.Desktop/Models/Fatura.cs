namespace ErpCrm.Desktop.Models;

/// <summary>
/// Fatura tipi enum
/// </summary>
public enum FaturaTipi
{
    Alis = 0,
    Satis = 1
}

/// <summary>
/// Fatura durumu enum
/// </summary>
public enum FaturaDurum
{
    Taslak = 0,
    Onaylandi = 1,
    Iptal = 2
}

/// <summary>
/// Fatura model sınıfı
/// </summary>
public class Fatura
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public string FaturaNo { get; set; } = string.Empty;
    public DateTime FaturaTarihi { get; set; }
    public string FaturaTipi { get; set; } = string.Empty;
    public int? CariId { get; set; }
    public string CariAdi { get; set; } = string.Empty;
    public decimal AraToplam { get; set; }
    public decimal KdvToplam { get; set; }
    public decimal IndirimToplam { get; set; }
    public decimal GenelToplam { get; set; }
    public string Durum { get; set; } = string.Empty;
    public string? Aciklama { get; set; }
    public DateTime OlusturmaTarihi { get; set; }

    /// <summary>
    /// Fatura tipinin Türkçe karşılığı
    /// </summary>
    public string FaturaTipiText => FaturaTipi switch
    {
        "Alis" => "Alış",
        "Satis" => "Satış",
        _ => FaturaTipi
    };

    /// <summary>
    /// Durum'un Türkçe karşılığı
    /// </summary>
    public string DurumText => Durum switch
    {
        "Taslak" => "Taslak",
        "Onaylandi" => "Onaylandı",
        "Iptal" => "İptal",
        _ => Durum
    };
}

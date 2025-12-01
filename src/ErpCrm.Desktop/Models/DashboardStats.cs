namespace ErpCrm.Desktop.Models;

/// <summary>
/// Dashboard istatistikleri model sınıfı
/// </summary>
public class DashboardStats
{
    public int CustomerCount { get; set; }
    public int ProductCount { get; set; }
    public int InvoiceCount { get; set; }
    public decimal TotalRevenue { get; set; }
    public int LowStockCount { get; set; }
    public List<RecentInvoice> RecentInvoices { get; set; } = new();
}

/// <summary>
/// Son fatura özeti model sınıfı
/// </summary>
public class RecentInvoice
{
    public int Id { get; set; }
    public string FaturaNo { get; set; } = string.Empty;
    public DateTime FaturaTarihi { get; set; }
    public string FaturaTipi { get; set; } = string.Empty;
    public string CariAdi { get; set; } = string.Empty;
    public decimal GenelToplam { get; set; }
    public string Durum { get; set; } = string.Empty;

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

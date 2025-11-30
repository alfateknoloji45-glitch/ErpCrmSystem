namespace ErpCrm.Core.Entities;

/// <summary>
/// Fatura satırı entity sınıfı
/// Faturanın her bir ürün/hizmet satırını tutar
/// </summary>
public class FaturaSatiri
{
    public int Id { get; set; }
    
    /// <summary>
    /// Bağlı fatura ID
    /// </summary>
    public int FaturaId { get; set; }
    
    /// <summary>
    /// Stok kartı ID
    /// </summary>
    public int StokId { get; set; }
    
    /// <summary>
    /// Miktar
    /// </summary>
    public decimal Miktar { get; set; }
    
    /// <summary>
    /// Birim fiyat
    /// </summary>
    public decimal BirimFiyat { get; set; }
    
    /// <summary>
    /// KDV oranı (%)
    /// </summary>
    public int KdvOrani { get; set; } = 18;
    
    /// <summary>
    /// KDV tutarı
    /// </summary>
    public decimal KdvTutar { get; set; } = 0;
    
    /// <summary>
    /// İndirim oranı (%)
    /// </summary>
    public decimal IndirimOrani { get; set; } = 0;
    
    /// <summary>
    /// İndirim tutarı
    /// </summary>
    public decimal IndirimTutar { get; set; } = 0;
    
    /// <summary>
    /// Toplam tutar (KDV dahil)
    /// </summary>
    public decimal ToplamTutar { get; set; } = 0;
    
    /// <summary>
    /// Satır açıklaması
    /// </summary>
    public string? Aciklama { get; set; }
    
    /// <summary>
    /// Sıra numarası
    /// </summary>
    public int SiraNo { get; set; } = 0;
    
    // Navigation Properties
    public virtual Fatura? Fatura { get; set; }
    public virtual StokKarti? Stok { get; set; }
}
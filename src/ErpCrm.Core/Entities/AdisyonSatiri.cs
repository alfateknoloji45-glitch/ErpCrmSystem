namespace ErpCrm.Core.Entities;

/// <summary>
/// Adisyon satırı entity sınıfı (POS)
/// Adisyonun her bir sipariş satırını tutar
/// </summary>
public class AdisyonSatiri
{
    public int Id { get; set; }
    
    /// <summary>
    /// Bağlı adisyon ID
    /// </summary>
    public int AdisyonId { get; set; }
    
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
    /// İndirim oranı (%)
    /// </summary>
    public decimal IndirimOrani { get; set; } = 0;
    
    /// <summary>
    /// İndirim tutarı
    /// </summary>
    public decimal IndirimTutar { get; set; } = 0;
    
    /// <summary>
    /// Toplam tutar
    /// </summary>
    public decimal ToplamTutar { get; set; } = 0;
    
    /// <summary>
    /// Not (müşteri isteği vb.)
    /// </summary>
    public string? Not { get; set; }
    
    /// <summary>
    /// Sıra numarası
    /// </summary>
    public int SiraNo { get; set; } = 0;
    
    /// <summary>
    /// Oluşturma tarihi
    /// </summary>
    public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;
    
    // Navigation Properties
    public virtual Adisyon? Adisyon { get; set; }
    public virtual StokKarti? Stok { get; set; }
}
-- ========================================
-- ERP/CRM/POS Sistem Veritabanı Şeması
-- Tam MSSQL Şeması
-- ========================================

-- Önce mevcut tabloları temizle (eğer varsa)
IF OBJECT_ID('dbo.AdisyonSatirlari', 'U') IS NOT NULL DROP TABLE dbo.AdisyonSatirlari;
IF OBJECT_ID('dbo.Adisyonlar', 'U') IS NOT NULL DROP TABLE dbo.Adisyonlar;
IF OBJECT_ID('dbo.Masalar', 'U') IS NOT NULL DROP TABLE dbo.Masalar;
IF OBJECT_ID('dbo.FaturaSatirlari', 'U') IS NOT NULL DROP TABLE dbo.FaturaSatirlari;
IF OBJECT_ID('dbo.Faturalar', 'U') IS NOT NULL DROP TABLE dbo.Faturalar;
IF OBJECT_ID('dbo.StokKartlari', 'U') IS NOT NULL DROP TABLE dbo.StokKartlari;
IF OBJECT_ID('dbo.Cariler', 'U') IS NOT NULL DROP TABLE dbo.Cariler;
IF OBJECT_ID('dbo.CrmAktiviteler', 'U') IS NOT NULL DROP TABLE dbo.CrmAktiviteler;
IF OBJECT_ID('dbo.CrmMusteriler', 'U') IS NOT NULL DROP TABLE dbo.CrmMusteriler;
IF OBJECT_ID('dbo.TenantModules', 'U') IS NOT NULL DROP TABLE dbo.TenantModules;
IF OBJECT_ID('dbo.PlanModules', 'U') IS NOT NULL DROP TABLE dbo.PlanModules;
IF OBJECT_ID('dbo.TenantSubscriptions', 'U') IS NOT NULL DROP TABLE dbo.TenantSubscriptions;
IF OBJECT_ID('dbo.Users', 'U') IS NOT NULL DROP TABLE dbo.Users;
IF OBJECT_ID('dbo.SubscriptionPlans', 'U') IS NOT NULL DROP TABLE dbo.SubscriptionPlans;
IF OBJECT_ID('dbo.Modules', 'U') IS NOT NULL DROP TABLE dbo.Modules;
IF OBJECT_ID('dbo.Tenants', 'U') IS NOT NULL DROP TABLE dbo.Tenants;
GO

-- ========================================
-- 1. Tenants (Firmalar) Tablosu
-- ========================================
CREATE TABLE Tenants (
    Id INT PRIMARY KEY IDENTITY(1,1),
    FirmaKodu NVARCHAR(20) NOT NULL,
    FirmaAdi NVARCHAR(200) NOT NULL,
    VergiNo NVARCHAR(20) NULL,
    Telefon NVARCHAR(20) NULL,
    Email NVARCHAR(100) NULL,
    Adres NVARCHAR(500) NULL,
    Durum INT NOT NULL DEFAULT 1, -- 0: Pasif, 1: Aktif, 2: Demo, 3: Askıda
    DemoMu BIT NOT NULL DEFAULT 0,
    DemoBitisTarihi DATETIME NULL,
    OlusturmaTarihi DATETIME NOT NULL DEFAULT GETDATE(),
    GuncellemeTarihi DATETIME NULL,
    CONSTRAINT UQ_Tenants_FirmaKodu UNIQUE (FirmaKodu)
);
GO

CREATE INDEX IX_Tenants_Durum ON Tenants(Durum);
CREATE INDEX IX_Tenants_DemoMu ON Tenants(DemoMu);
GO

-- ========================================
-- 2. Modules (Modüller) Tablosu
-- ========================================
CREATE TABLE Modules (
    Id INT PRIMARY KEY IDENTITY(1,1),
    ModulKodu NVARCHAR(50) NOT NULL,
    ModulAdi NVARCHAR(100) NOT NULL,
    Aciklama NVARCHAR(500) NULL,
    AylikUcret DECIMAL(18, 2) NOT NULL DEFAULT 0,
    YillikUcret DECIMAL(18, 2) NOT NULL DEFAULT 0,
    Kategori NVARCHAR(50) NULL,
    Aktif BIT NOT NULL DEFAULT 1,
    OlusturmaTarihi DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT UQ_Modules_ModulKodu UNIQUE (ModulKodu)
);
GO

CREATE INDEX IX_Modules_Kategori ON Modules(Kategori);
CREATE INDEX IX_Modules_Aktif ON Modules(Aktif);
GO

-- ========================================
-- 3. SubscriptionPlans (Abonelik Planları) Tablosu
-- ========================================
CREATE TABLE SubscriptionPlans (
    Id INT PRIMARY KEY IDENTITY(1,1),
    PlanKodu NVARCHAR(50) NOT NULL,
    PlanAdi NVARCHAR(100) NOT NULL,
    Aciklama NVARCHAR(500) NULL,
    AylikUcret DECIMAL(18, 2) NOT NULL DEFAULT 0,
    YillikUcret DECIMAL(18, 2) NOT NULL DEFAULT 0,
    MaxKullanici INT NOT NULL DEFAULT 1,
    Aktif BIT NOT NULL DEFAULT 1,
    OlusturmaTarihi DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT UQ_SubscriptionPlans_PlanKodu UNIQUE (PlanKodu)
);
GO

-- ========================================
-- 4. Users (Kullanıcılar) Tablosu
-- ========================================
CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    TenantId INT NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    PasswordHash NVARCHAR(256) NOT NULL,
    AdSoyad NVARCHAR(100) NOT NULL,
    Telefon NVARCHAR(20) NULL,
    Rol INT NOT NULL DEFAULT 2, -- 0: SuperAdmin, 1: TenantAdmin, 2: User, 3: Garson, 4: Kasiyer
    Aktif BIT NOT NULL DEFAULT 1,
    SonGirisTarihi DATETIME NULL,
    OlusturmaTarihi DATETIME NOT NULL DEFAULT GETDATE(),
    GuncellemeTarihi DATETIME NULL,
    CONSTRAINT FK_Users_Tenants FOREIGN KEY (TenantId) REFERENCES Tenants(Id) ON DELETE CASCADE,
    CONSTRAINT UQ_Users_Email UNIQUE (Email)
);
GO

CREATE INDEX IX_Users_TenantId ON Users(TenantId);
CREATE INDEX IX_Users_Rol ON Users(Rol);
CREATE INDEX IX_Users_Aktif ON Users(Aktif);
GO

-- ========================================
-- 5. TenantSubscriptions (Firma Abonelikleri) Tablosu
-- ========================================
CREATE TABLE TenantSubscriptions (
    Id INT PRIMARY KEY IDENTITY(1,1),
    TenantId INT NOT NULL,
    SubscriptionPlanId INT NOT NULL,
    BaslangicTarihi DATETIME NOT NULL,
    BitisTarihi DATETIME NOT NULL,
    OdemeTipi INT NOT NULL DEFAULT 1, -- 1: Aylık, 2: Yıllık
    Durum INT NOT NULL DEFAULT 1, -- 0: Pasif, 1: Aktif, 2: Süresi Dolmuş
    OlusturmaTarihi DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_TenantSubscriptions_Tenants FOREIGN KEY (TenantId) REFERENCES Tenants(Id) ON DELETE CASCADE,
    CONSTRAINT FK_TenantSubscriptions_Plans FOREIGN KEY (SubscriptionPlanId) REFERENCES SubscriptionPlans(Id)
);
GO

CREATE INDEX IX_TenantSubscriptions_TenantId ON TenantSubscriptions(TenantId);
CREATE INDEX IX_TenantSubscriptions_Durum ON TenantSubscriptions(Durum);
GO

-- ========================================
-- 6. PlanModules (Plan-Modül İlişkisi) Tablosu
-- ========================================
CREATE TABLE PlanModules (
    Id INT PRIMARY KEY IDENTITY(1,1),
    SubscriptionPlanId INT NOT NULL,
    ModuleId INT NOT NULL,
    OlusturmaTarihi DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_PlanModules_Plans FOREIGN KEY (SubscriptionPlanId) REFERENCES SubscriptionPlans(Id) ON DELETE CASCADE,
    CONSTRAINT FK_PlanModules_Modules FOREIGN KEY (ModuleId) REFERENCES Modules(Id) ON DELETE CASCADE,
    CONSTRAINT UQ_PlanModules UNIQUE (SubscriptionPlanId, ModuleId)
);
GO

-- ========================================
-- 7. TenantModules (Firma-Modül İlişkisi) Tablosu
-- ========================================
CREATE TABLE TenantModules (
    Id INT PRIMARY KEY IDENTITY(1,1),
    TenantId INT NOT NULL,
    ModuleId INT NOT NULL,
    Aktif BIT NOT NULL DEFAULT 1,
    OlusturmaTarihi DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_TenantModules_Tenants FOREIGN KEY (TenantId) REFERENCES Tenants(Id) ON DELETE CASCADE,
    CONSTRAINT FK_TenantModules_Modules FOREIGN KEY (ModuleId) REFERENCES Modules(Id) ON DELETE CASCADE,
    CONSTRAINT UQ_TenantModules UNIQUE (TenantId, ModuleId)
);
GO

-- ========================================
-- 8. Cariler (Cari Hesaplar) Tablosu
-- ========================================
CREATE TABLE Cariler (
    Id INT PRIMARY KEY IDENTITY(1,1),
    TenantId INT NOT NULL,
    CariKodu NVARCHAR(50) NOT NULL,
    CariAdi NVARCHAR(200) NOT NULL,
    CariTip INT NOT NULL DEFAULT 0, -- 0: Müşteri, 1: Tedarikçi, 2: Her İkisi
    VergiDairesi NVARCHAR(100) NULL,
    VergiNo NVARCHAR(20) NULL,
    Telefon NVARCHAR(20) NULL,
    Email NVARCHAR(100) NULL,
    Adres NVARCHAR(500) NULL,
    Il NVARCHAR(50) NULL,
    Ilce NVARCHAR(50) NULL,
    Bakiye DECIMAL(18, 2) NOT NULL DEFAULT 0,
    AlacakLimiti DECIMAL(18, 2) NOT NULL DEFAULT 0,
    Aktif BIT NOT NULL DEFAULT 1,
    OlusturmaTarihi DATETIME NOT NULL DEFAULT GETDATE(),
    GuncellemeTarihi DATETIME NULL,
    CONSTRAINT FK_Cariler_Tenants FOREIGN KEY (TenantId) REFERENCES Tenants(Id) ON DELETE CASCADE,
    CONSTRAINT UQ_Cariler_TenantId_CariKodu UNIQUE (TenantId, CariKodu)
);
GO

CREATE INDEX IX_Cariler_TenantId ON Cariler(TenantId);
CREATE INDEX IX_Cariler_CariTip ON Cariler(CariTip);
CREATE INDEX IX_Cariler_CariAdi ON Cariler(CariAdi);
GO

-- ========================================
-- 9. StokKartlari (Stok Kartları) Tablosu
-- ========================================
CREATE TABLE StokKartlari (
    Id INT PRIMARY KEY IDENTITY(1,1),
    TenantId INT NOT NULL,
    StokKodu NVARCHAR(50) NOT NULL,
    StokAdi NVARCHAR(200) NOT NULL,
    Barkod NVARCHAR(50) NULL,
    Birim NVARCHAR(20) NOT NULL DEFAULT 'Adet',
    Kategori NVARCHAR(100) NULL,
    AltKategori NVARCHAR(100) NULL,
    AlisFiyati DECIMAL(18, 2) NOT NULL DEFAULT 0,
    SatisFiyati DECIMAL(18, 2) NOT NULL DEFAULT 0,
    KdvOrani INT NOT NULL DEFAULT 18,
    StokMiktari DECIMAL(18, 3) NOT NULL DEFAULT 0,
    MinStokMiktari DECIMAL(18, 3) NOT NULL DEFAULT 0,
    Aciklama NVARCHAR(500) NULL,
    Aktif BIT NOT NULL DEFAULT 1,
    OlusturmaTarihi DATETIME NOT NULL DEFAULT GETDATE(),
    GuncellemeTarihi DATETIME NULL,
    CONSTRAINT FK_StokKartlari_Tenants FOREIGN KEY (TenantId) REFERENCES Tenants(Id) ON DELETE CASCADE,
    CONSTRAINT UQ_StokKartlari_TenantId_StokKodu UNIQUE (TenantId, StokKodu)
);
GO

CREATE INDEX IX_StokKartlari_TenantId ON StokKartlari(TenantId);
CREATE INDEX IX_StokKartlari_Barkod ON StokKartlari(Barkod);
CREATE INDEX IX_StokKartlari_Kategori ON StokKartlari(Kategori);
CREATE INDEX IX_StokKartlari_StokAdi ON StokKartlari(StokAdi);
GO

-- ========================================
-- 10. Faturalar Tablosu
-- ========================================
CREATE TABLE Faturalar (
    Id INT PRIMARY KEY IDENTITY(1,1),
    TenantId INT NOT NULL,
    FaturaNo NVARCHAR(50) NOT NULL,
    FaturaTarihi DATETIME NOT NULL,
    FaturaTipi INT NOT NULL DEFAULT 1, -- 0: Alış, 1: Satış
    CariId INT NULL,
    AraToplam DECIMAL(18, 2) NOT NULL DEFAULT 0,
    KdvToplam DECIMAL(18, 2) NOT NULL DEFAULT 0,
    IndirimToplam DECIMAL(18, 2) NOT NULL DEFAULT 0,
    GenelToplam DECIMAL(18, 2) NOT NULL DEFAULT 0,
    Durum INT NOT NULL DEFAULT 0, -- 0: Taslak, 1: Onaylandı, 2: İptal
    Aciklama NVARCHAR(500) NULL,
    OlusturanKullaniciId INT NULL,
    OlusturmaTarihi DATETIME NOT NULL DEFAULT GETDATE(),
    GuncellemeTarihi DATETIME NULL,
    CONSTRAINT FK_Faturalar_Tenants FOREIGN KEY (TenantId) REFERENCES Tenants(Id) ON DELETE CASCADE,
    CONSTRAINT FK_Faturalar_Cariler FOREIGN KEY (CariId) REFERENCES Cariler(Id),
    CONSTRAINT FK_Faturalar_Users FOREIGN KEY (OlusturanKullaniciId) REFERENCES Users(Id),
    CONSTRAINT UQ_Faturalar_TenantId_FaturaNo UNIQUE (TenantId, FaturaNo)
);
GO

CREATE INDEX IX_Faturalar_TenantId ON Faturalar(TenantId);
CREATE INDEX IX_Faturalar_FaturaTarihi ON Faturalar(FaturaTarihi);
CREATE INDEX IX_Faturalar_CariId ON Faturalar(CariId);
CREATE INDEX IX_Faturalar_Durum ON Faturalar(Durum);
GO

-- ========================================
-- 11. FaturaSatirlari Tablosu
-- ========================================
CREATE TABLE FaturaSatirlari (
    Id INT PRIMARY KEY IDENTITY(1,1),
    FaturaId INT NOT NULL,
    StokId INT NOT NULL,
    Miktar DECIMAL(18, 3) NOT NULL,
    BirimFiyat DECIMAL(18, 2) NOT NULL,
    KdvOrani INT NOT NULL DEFAULT 18,
    KdvTutar DECIMAL(18, 2) NOT NULL DEFAULT 0,
    IndirimOrani DECIMAL(5, 2) NOT NULL DEFAULT 0,
    IndirimTutar DECIMAL(18, 2) NOT NULL DEFAULT 0,
    ToplamTutar DECIMAL(18, 2) NOT NULL DEFAULT 0,
    Aciklama NVARCHAR(200) NULL,
    SiraNo INT NOT NULL DEFAULT 0,
    CONSTRAINT FK_FaturaSatirlari_Faturalar FOREIGN KEY (FaturaId) REFERENCES Faturalar(Id) ON DELETE CASCADE,
    CONSTRAINT FK_FaturaSatirlari_StokKartlari FOREIGN KEY (StokId) REFERENCES StokKartlari(Id)
);
GO

CREATE INDEX IX_FaturaSatirlari_FaturaId ON FaturaSatirlari(FaturaId);
CREATE INDEX IX_FaturaSatirlari_StokId ON FaturaSatirlari(StokId);
GO

-- ========================================
-- 12. Masalar (POS) Tablosu
-- ========================================
CREATE TABLE Masalar (
    Id INT PRIMARY KEY IDENTITY(1,1),
    TenantId INT NOT NULL,
    MasaNo NVARCHAR(20) NOT NULL,
    MasaAdi NVARCHAR(50) NULL,
    Kapasite INT NOT NULL DEFAULT 4,
    Bolum NVARCHAR(50) NULL, -- Salon, Bahçe, VIP vb.
    Durum INT NOT NULL DEFAULT 0, -- 0: Boş, 1: Dolu, 2: Rezerve
    Aktif BIT NOT NULL DEFAULT 1,
    OlusturmaTarihi DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Masalar_Tenants FOREIGN KEY (TenantId) REFERENCES Tenants(Id) ON DELETE CASCADE,
    CONSTRAINT UQ_Masalar_TenantId_MasaNo UNIQUE (TenantId, MasaNo)
);
GO

CREATE INDEX IX_Masalar_TenantId ON Masalar(TenantId);
CREATE INDEX IX_Masalar_Durum ON Masalar(Durum);
GO

-- ========================================
-- 13. Adisyonlar (POS) Tablosu
-- ========================================
CREATE TABLE Adisyonlar (
    Id INT PRIMARY KEY IDENTITY(1,1),
    TenantId INT NOT NULL,
    MasaId INT NOT NULL,
    AdisyonNo NVARCHAR(50) NOT NULL,
    AcilisTarihi DATETIME NOT NULL DEFAULT GETDATE(),
    KapanisTarihi DATETIME NULL,
    GarsonId INT NULL,
    AraToplam DECIMAL(18, 2) NOT NULL DEFAULT 0,
    IndirimToplam DECIMAL(18, 2) NOT NULL DEFAULT 0,
    GenelToplam DECIMAL(18, 2) NOT NULL DEFAULT 0,
    OdenenTutar DECIMAL(18, 2) NOT NULL DEFAULT 0,
    Durum INT NOT NULL DEFAULT 0, -- 0: Açık, 1: Kapalı, 2: İptal
    Aciklama NVARCHAR(200) NULL,
    OlusturmaTarihi DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Adisyonlar_Tenants FOREIGN KEY (TenantId) REFERENCES Tenants(Id) ON DELETE CASCADE,
    CONSTRAINT FK_Adisyonlar_Masalar FOREIGN KEY (MasaId) REFERENCES Masalar(Id),
    CONSTRAINT FK_Adisyonlar_Users FOREIGN KEY (GarsonId) REFERENCES Users(Id),
    CONSTRAINT UQ_Adisyonlar_TenantId_AdisyonNo UNIQUE (TenantId, AdisyonNo)
);
GO

CREATE INDEX IX_Adisyonlar_TenantId ON Adisyonlar(TenantId);
CREATE INDEX IX_Adisyonlar_MasaId ON Adisyonlar(MasaId);
CREATE INDEX IX_Adisyonlar_Durum ON Adisyonlar(Durum);
CREATE INDEX IX_Adisyonlar_AcilisTarihi ON Adisyonlar(AcilisTarihi);
GO

-- ========================================
-- 14. AdisyonSatirlari (POS) Tablosu
-- ========================================
CREATE TABLE AdisyonSatirlari (
    Id INT PRIMARY KEY IDENTITY(1,1),
    AdisyonId INT NOT NULL,
    StokId INT NOT NULL,
    Miktar DECIMAL(18, 3) NOT NULL,
    BirimFiyat DECIMAL(18, 2) NOT NULL,
    IndirimOrani DECIMAL(5, 2) NOT NULL DEFAULT 0,
    IndirimTutar DECIMAL(18, 2) NOT NULL DEFAULT 0,
    ToplamTutar DECIMAL(18, 2) NOT NULL DEFAULT 0,
    Not1 NVARCHAR(200) NULL,
    SiraNo INT NOT NULL DEFAULT 0,
    OlusturmaTarihi DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_AdisyonSatirlari_Adisyonlar FOREIGN KEY (AdisyonId) REFERENCES Adisyonlar(Id) ON DELETE CASCADE,
    CONSTRAINT FK_AdisyonSatirlari_StokKartlari FOREIGN KEY (StokId) REFERENCES StokKartlari(Id)
);
GO

CREATE INDEX IX_AdisyonSatirlari_AdisyonId ON AdisyonSatirlari(AdisyonId);
CREATE INDEX IX_AdisyonSatirlari_StokId ON AdisyonSatirlari(StokId);
GO

-- ========================================
-- 15. CrmMusteriler (CRM) Tablosu
-- ========================================
CREATE TABLE CrmMusteriler (
    Id INT PRIMARY KEY IDENTITY(1,1),
    TenantId INT NOT NULL,
    MusteriKodu NVARCHAR(50) NOT NULL,
    MusteriAdi NVARCHAR(200) NOT NULL,
    FirmaAdi NVARCHAR(200) NULL,
    Telefon NVARCHAR(20) NULL,
    Email NVARCHAR(100) NULL,
    Adres NVARCHAR(500) NULL,
    Il NVARCHAR(50) NULL,
    Ilce NVARCHAR(50) NULL,
    Sektor NVARCHAR(100) NULL,
    MusteriKaynagi NVARCHAR(100) NULL, -- Web, Referans, Reklam vb.
    MusteriDurumu NVARCHAR(50) NULL, -- Lead, Prospect, Customer vb.
    AtananKullaniciId INT NULL,
    Not1 NVARCHAR(1000) NULL,
    Aktif BIT NOT NULL DEFAULT 1,
    OlusturmaTarihi DATETIME NOT NULL DEFAULT GETDATE(),
    GuncellemeTarihi DATETIME NULL,
    CONSTRAINT FK_CrmMusteriler_Tenants FOREIGN KEY (TenantId) REFERENCES Tenants(Id) ON DELETE CASCADE,
    CONSTRAINT FK_CrmMusteriler_Users FOREIGN KEY (AtananKullaniciId) REFERENCES Users(Id),
    CONSTRAINT UQ_CrmMusteriler_TenantId_MusteriKodu UNIQUE (TenantId, MusteriKodu)
);
GO

CREATE INDEX IX_CrmMusteriler_TenantId ON CrmMusteriler(TenantId);
CREATE INDEX IX_CrmMusteriler_MusteriDurumu ON CrmMusteriler(MusteriDurumu);
CREATE INDEX IX_CrmMusteriler_AtananKullaniciId ON CrmMusteriler(AtananKullaniciId);
GO

-- ========================================
-- 16. CrmAktiviteler (CRM) Tablosu
-- ========================================
CREATE TABLE CrmAktiviteler (
    Id INT PRIMARY KEY IDENTITY(1,1),
    TenantId INT NOT NULL,
    CrmMusteriId INT NOT NULL,
    AktiviteTipi NVARCHAR(50) NOT NULL, -- Arama, Toplantı, Email, Teklif vb.
    Baslik NVARCHAR(200) NOT NULL,
    Aciklama NVARCHAR(1000) NULL,
    PlanlananTarih DATETIME NULL,
    TamamlanmaTarihi DATETIME NULL,
    Durum NVARCHAR(50) NOT NULL DEFAULT 'Planlandı', -- Planlandı, Tamamlandı, İptal
    SorumluKullaniciId INT NULL,
    Oncelik INT NOT NULL DEFAULT 2, -- 1: Düşük, 2: Normal, 3: Yüksek
    OlusturmaTarihi DATETIME NOT NULL DEFAULT GETDATE(),
    GuncellemeTarihi DATETIME NULL,
    CONSTRAINT FK_CrmAktiviteler_Tenants FOREIGN KEY (TenantId) REFERENCES Tenants(Id) ON DELETE CASCADE,
    CONSTRAINT FK_CrmAktiviteler_CrmMusteriler FOREIGN KEY (CrmMusteriId) REFERENCES CrmMusteriler(Id),
    CONSTRAINT FK_CrmAktiviteler_Users FOREIGN KEY (SorumluKullaniciId) REFERENCES Users(Id)
);
GO

CREATE INDEX IX_CrmAktiviteler_TenantId ON CrmAktiviteler(TenantId);
CREATE INDEX IX_CrmAktiviteler_CrmMusteriId ON CrmAktiviteler(CrmMusteriId);
CREATE INDEX IX_CrmAktiviteler_PlanlananTarih ON CrmAktiviteler(PlanlananTarih);
CREATE INDEX IX_CrmAktiviteler_Durum ON CrmAktiviteler(Durum);
GO

-- ========================================
-- ÖRNEK VERİLER
-- ========================================

-- 1. Modüller
INSERT INTO Modules (ModulKodu, ModulAdi, Aciklama, AylikUcret, YillikUcret, Kategori, Aktif) VALUES
('CARI', 'Cari Yönetimi', 'Müşteri ve tedarikçi cari hesap yönetimi', 100.00, 1000.00, 'ERP', 1),
('STOK', 'Stok Yönetimi', 'Stok kartları ve stok takibi', 150.00, 1500.00, 'ERP', 1),
('FATURA', 'Fatura Yönetimi', 'Alış ve satış fatura yönetimi', 200.00, 2000.00, 'ERP', 1),
('POS', 'Satış Noktası', 'Restoran ve cafe POS sistemi', 250.00, 2500.00, 'POS', 1),
('CRM', 'Müşteri İlişkileri', 'CRM ve aktivite takibi', 175.00, 1750.00, 'CRM', 1),
('RAPORLAMA', 'Raporlama', 'Detaylı raporlar ve analizler', 100.00, 1000.00, 'GENEL', 1);
GO

-- 2. Abonelik Planları
INSERT INTO SubscriptionPlans (PlanKodu, PlanAdi, Aciklama, AylikUcret, YillikUcret, MaxKullanici, Aktif) VALUES
('STARTER', 'Başlangıç Paketi', 'Küçük işletmeler için temel paket', 299.00, 2990.00, 3, 1),
('PROFESSIONAL', 'Profesyonel Paket', 'Orta ölçekli işletmeler için', 599.00, 5990.00, 10, 1),
('ENTERPRISE', 'Kurumsal Paket', 'Büyük işletmeler için tam paket', 999.00, 9990.00, 50, 1),
('DEMO', 'Demo Paket', '14 günlük ücretsiz deneme', 0.00, 0.00, 5, 1);
GO

-- 3. Plan-Modül İlişkileri
-- Başlangıç Paketi: CARI, STOK
INSERT INTO PlanModules (SubscriptionPlanId, ModuleId) VALUES (1, 1), (1, 2);
-- Profesyonel Paket: CARI, STOK, FATURA, RAPORLAMA
INSERT INTO PlanModules (SubscriptionPlanId, ModuleId) VALUES (2, 1), (2, 2), (2, 3), (2, 6);
-- Kurumsal Paket: Tüm modüller
INSERT INTO PlanModules (SubscriptionPlanId, ModuleId) VALUES (3, 1), (3, 2), (3, 3), (3, 4), (3, 5), (3, 6);
-- Demo Paket: Tüm modüller
INSERT INTO PlanModules (SubscriptionPlanId, ModuleId) VALUES (4, 1), (4, 2), (4, 3), (4, 4), (4, 5), (4, 6);
GO

-- 4. Demo Firma
INSERT INTO Tenants (FirmaKodu, FirmaAdi, VergiNo, Telefon, Email, Adres, Durum, DemoMu, DemoBitisTarihi) VALUES
('DEMO001', 'Demo Firma A.Ş.', '1234567890', '0212 555 0001', 'demo@demofirma.com', 'Atatürk Cad. No:1 İstanbul', 2, 1, DATEADD(DAY, 14, GETDATE()));
GO

-- 5. Demo Kullanıcıları (Şifre: Demo123! - SHA256 hash: jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=)
INSERT INTO Users (TenantId, Email, PasswordHash, AdSoyad, Telefon, Rol, Aktif) VALUES
(1, 'admin@demofirma.com', 'jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=', 'Demo Admin', '0532 555 0001', 1, 1),
(1, 'user@demofirma.com', 'jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=', 'Demo Kullanıcı', '0532 555 0002', 2, 1),
(1, 'garson@demofirma.com', 'jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=', 'Demo Garson', '0532 555 0003', 3, 1),
(1, 'kasiyer@demofirma.com', 'jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=', 'Demo Kasiyer', '0532 555 0004', 4, 1);
GO

-- 6. Demo Firma Aboneliği
INSERT INTO TenantSubscriptions (TenantId, SubscriptionPlanId, BaslangicTarihi, BitisTarihi, OdemeTipi, Durum) VALUES
(1, 4, GETDATE(), DATEADD(DAY, 14, GETDATE()), 1, 1);
GO

-- 7. Demo Firma Modülleri
INSERT INTO TenantModules (TenantId, ModuleId, Aktif) VALUES
(1, 1, 1), (1, 2, 1), (1, 3, 1), (1, 4, 1), (1, 5, 1), (1, 6, 1);
GO

-- 8. Örnek Cariler
INSERT INTO Cariler (TenantId, CariKodu, CariAdi, CariTip, VergiDairesi, VergiNo, Telefon, Email, Adres, Il, Bakiye, Aktif) VALUES
(1, 'C001', 'ABC Ticaret Ltd.', 0, 'Beşiktaş', '1234567891', '0212 444 1111', 'info@abcticaret.com', 'Barbaros Bulvarı No:10', 'İstanbul', 5000.00, 1),
(1, 'C002', 'XYZ Tedarik A.Ş.', 1, 'Kadıköy', '9876543210', '0216 333 2222', 'satis@xyztedarik.com', 'Bağdat Caddesi No:200', 'İstanbul', -3500.00, 1),
(1, 'C003', 'Mehmet Yılmaz', 0, NULL, NULL, '0532 111 2233', 'mehmet@email.com', 'Merkez Mahallesi Çiçek Sokak No:5', 'Ankara', 1250.00, 1);
GO

-- 9. Örnek Stok Kartları
INSERT INTO StokKartlari (TenantId, StokKodu, StokAdi, Barkod, Birim, Kategori, AltKategori, AlisFiyati, SatisFiyati, KdvOrani, StokMiktari, MinStokMiktari, Aktif) VALUES
(1, 'S001', 'Çay (Bardak)', '8690000000001', 'Adet', 'İçecekler', 'Sıcak İçecekler', 2.00, 10.00, 8, 1000, 100, 1),
(1, 'S002', 'Türk Kahvesi', '8690000000002', 'Adet', 'İçecekler', 'Sıcak İçecekler', 5.00, 25.00, 8, 500, 50, 1),
(1, 'S003', 'Tost (Kaşarlı)', '8690000000003', 'Adet', 'Yiyecekler', 'Tostlar', 15.00, 45.00, 8, 200, 20, 1),
(1, 'S004', 'Hamburger Menü', '8690000000004', 'Adet', 'Yiyecekler', 'Menüler', 40.00, 120.00, 8, 100, 10, 1),
(1, 'S005', 'Kola (330ml)', '8690000000005', 'Adet', 'İçecekler', 'Soğuk İçecekler', 8.00, 25.00, 8, 500, 50, 1),
(1, 'S006', 'Laptop Bilgisayar', '8690000000006', 'Adet', 'Elektronik', 'Bilgisayar', 15000.00, 22000.00, 18, 25, 5, 1),
(1, 'S007', 'Ofis Sandalyesi', '8690000000007', 'Adet', 'Mobilya', 'Ofis', 800.00, 1500.00, 18, 50, 10, 1);
GO

-- 10. Örnek Masalar
INSERT INTO Masalar (TenantId, MasaNo, MasaAdi, Kapasite, Bolum, Durum, Aktif) VALUES
(1, '01', 'Masa 1', 4, 'Salon', 0, 1),
(1, '02', 'Masa 2', 4, 'Salon', 0, 1),
(1, '03', 'Masa 3', 6, 'Salon', 0, 1),
(1, '04', 'Masa 4', 2, 'Salon', 0, 1),
(1, 'B1', 'Bahçe 1', 4, 'Bahçe', 0, 1),
(1, 'B2', 'Bahçe 2', 6, 'Bahçe', 0, 1),
(1, 'V1', 'VIP 1', 8, 'VIP', 0, 1);
GO

-- 11. Örnek CRM Müşterileri
INSERT INTO CrmMusteriler (TenantId, MusteriKodu, MusteriAdi, FirmaAdi, Telefon, Email, Il, Sektor, MusteriKaynagi, MusteriDurumu, AtananKullaniciId, Aktif) VALUES
(1, 'CRM001', 'Ahmet Kaya', 'Kaya Holding', '0532 111 0001', 'ahmet@kayaholding.com', 'İstanbul', 'Finans', 'Web', 'Customer', 1, 1),
(1, 'CRM002', 'Ayşe Demir', 'Demir İnşaat', '0532 222 0002', 'ayse@demirinşaat.com', 'Ankara', 'İnşaat', 'Referans', 'Prospect', 1, 1),
(1, 'CRM003', 'Can Öztürk', 'Öztürk Yazılım', '0532 333 0003', 'can@ozturkyazilim.com', 'İzmir', 'Teknoloji', 'Reklam', 'Lead', 2, 1);
GO

-- 12. Örnek CRM Aktiviteleri
INSERT INTO CrmAktiviteler (TenantId, CrmMusteriId, AktiviteTipi, Baslik, Aciklama, PlanlananTarih, Durum, SorumluKullaniciId, Oncelik) VALUES
(1, 1, 'Toplantı', 'Yıllık sözleşme görüşmesi', 'Yıllık sözleşme yenileme toplantısı', DATEADD(DAY, 3, GETDATE()), 'Planlandı', 1, 3),
(1, 2, 'Arama', 'İlk temas araması', 'Potansiyel müşteri ile ilk görüşme', DATEADD(DAY, 1, GETDATE()), 'Planlandı', 1, 2),
(1, 3, 'Email', 'Teklif gönderimi', 'Yazılım çözümü teklifi gönderilecek', DATEADD(DAY, 2, GETDATE()), 'Planlandı', 2, 2);
GO

PRINT 'Veritabanı şeması ve örnek veriler başarıyla oluşturuldu!';
GO

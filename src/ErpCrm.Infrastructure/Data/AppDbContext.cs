using Microsoft.EntityFrameworkCore;
using ErpCrm.Core.Entities;

namespace ErpCrm.Infrastructure.Data;

/// <summary>
/// Entity Framework DbContext sınıfı
/// Tüm veritabanı işlemlerini yönetir
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // DbSet'ler
    public DbSet<Tenant> Tenants { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Module> Modules { get; set; } = null!;
    public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; } = null!;
    public DbSet<TenantSubscription> TenantSubscriptions { get; set; } = null!;
    public DbSet<PlanModule> PlanModules { get; set; } = null!;
    public DbSet<TenantModule> TenantModules { get; set; } = null!;
    public DbSet<Cari> Cariler { get; set; } = null!;
    public DbSet<StokKarti> StokKartlari { get; set; } = null!;
    public DbSet<Fatura> Faturalar { get; set; } = null!;
    public DbSet<FaturaSatiri> FaturaSatirlari { get; set; } = null!;
    public DbSet<Masa> Masalar { get; set; } = null!;
    public DbSet<Adisyon> Adisyonlar { get; set; } = null!;
    public DbSet<AdisyonSatiri> AdisyonSatirlari { get; set; } = null!;
    public DbSet<CrmMusteri> CrmMusteriler { get; set; } = null!;
    public DbSet<CrmAktivite> CrmAktiviteler { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Tenant Configuration
        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.ToTable("Tenants");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirmaKodu).HasMaxLength(20).IsRequired();
            entity.Property(e => e.FirmaAdi).HasMaxLength(200).IsRequired();
            entity.Property(e => e.VergiNo).HasMaxLength(20);
            entity.Property(e => e.Telefon).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Adres).HasMaxLength(500);
            entity.HasIndex(e => e.FirmaKodu).IsUnique();
            entity.HasIndex(e => e.Durum);
            entity.HasIndex(e => e.DemoMu);
        });

        // User Configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).HasMaxLength(100).IsRequired();
            entity.Property(e => e.PasswordHash).HasMaxLength(256).IsRequired();
            entity.Property(e => e.AdSoyad).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Telefon).HasMaxLength(20);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => e.Rol);
            entity.HasIndex(e => e.Aktif);

            entity.HasOne(e => e.Tenant)
                  .WithMany(t => t.Users)
                  .HasForeignKey(e => e.TenantId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Module Configuration
        modelBuilder.Entity<Module>(entity =>
        {
            entity.ToTable("Modules");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ModulKodu).HasMaxLength(50).IsRequired();
            entity.Property(e => e.ModulAdi).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Aciklama).HasMaxLength(500);
            entity.Property(e => e.AylikUcret).HasColumnType("decimal(18,2)");
            entity.Property(e => e.YillikUcret).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Kategori).HasMaxLength(50);
            entity.HasIndex(e => e.ModulKodu).IsUnique();
            entity.HasIndex(e => e.Kategori);
            entity.HasIndex(e => e.Aktif);
        });

        // SubscriptionPlan Configuration
        modelBuilder.Entity<SubscriptionPlan>(entity =>
        {
            entity.ToTable("SubscriptionPlans");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PlanKodu).HasMaxLength(50).IsRequired();
            entity.Property(e => e.PlanAdi).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Aciklama).HasMaxLength(500);
            entity.Property(e => e.AylikUcret).HasColumnType("decimal(18,2)");
            entity.Property(e => e.YillikUcret).HasColumnType("decimal(18,2)");
            entity.HasIndex(e => e.PlanKodu).IsUnique();
        });

        // TenantSubscription Configuration
        modelBuilder.Entity<TenantSubscription>(entity =>
        {
            entity.ToTable("TenantSubscriptions");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => e.Durum);

            entity.HasOne(e => e.Tenant)
                  .WithMany(t => t.Subscriptions)
                  .HasForeignKey(e => e.TenantId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.SubscriptionPlan)
                  .WithMany(p => p.TenantSubscriptions)
                  .HasForeignKey(e => e.SubscriptionPlanId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // PlanModule Configuration
        modelBuilder.Entity<PlanModule>(entity =>
        {
            entity.ToTable("PlanModules");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.SubscriptionPlanId, e.ModuleId }).IsUnique();

            entity.HasOne(e => e.SubscriptionPlan)
                  .WithMany(p => p.PlanModules)
                  .HasForeignKey(e => e.SubscriptionPlanId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Module)
                  .WithMany(m => m.PlanModules)
                  .HasForeignKey(e => e.ModuleId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // TenantModule Configuration
        modelBuilder.Entity<TenantModule>(entity =>
        {
            entity.ToTable("TenantModules");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.TenantId, e.ModuleId }).IsUnique();

            entity.HasOne(e => e.Tenant)
                  .WithMany(t => t.TenantModules)
                  .HasForeignKey(e => e.TenantId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Module)
                  .WithMany(m => m.TenantModules)
                  .HasForeignKey(e => e.ModuleId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Cari Configuration
        modelBuilder.Entity<Cari>(entity =>
        {
            entity.ToTable("Cariler");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CariKodu).HasMaxLength(50).IsRequired();
            entity.Property(e => e.CariAdi).HasMaxLength(200).IsRequired();
            entity.Property(e => e.VergiDairesi).HasMaxLength(100);
            entity.Property(e => e.VergiNo).HasMaxLength(20);
            entity.Property(e => e.Telefon).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Adres).HasMaxLength(500);
            entity.Property(e => e.Il).HasMaxLength(50);
            entity.Property(e => e.Ilce).HasMaxLength(50);
            entity.Property(e => e.Bakiye).HasColumnType("decimal(18,2)");
            entity.Property(e => e.AlacakLimiti).HasColumnType("decimal(18,2)");
            entity.HasIndex(e => new { e.TenantId, e.CariKodu }).IsUnique();
            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => e.CariTip);
            entity.HasIndex(e => e.CariAdi);

            entity.HasOne(e => e.Tenant)
                  .WithMany(t => t.Cariler)
                  .HasForeignKey(e => e.TenantId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // StokKarti Configuration
        modelBuilder.Entity<StokKarti>(entity =>
        {
            entity.ToTable("StokKartlari");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.StokKodu).HasMaxLength(50).IsRequired();
            entity.Property(e => e.StokAdi).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Barkod).HasMaxLength(50);
            entity.Property(e => e.Birim).HasMaxLength(20);
            entity.Property(e => e.Kategori).HasMaxLength(100);
            entity.Property(e => e.AltKategori).HasMaxLength(100);
            entity.Property(e => e.AlisFiyati).HasColumnType("decimal(18,2)");
            entity.Property(e => e.SatisFiyati).HasColumnType("decimal(18,2)");
            entity.Property(e => e.StokMiktari).HasColumnType("decimal(18,3)");
            entity.Property(e => e.MinStokMiktari).HasColumnType("decimal(18,3)");
            entity.Property(e => e.Aciklama).HasMaxLength(500);
            entity.HasIndex(e => new { e.TenantId, e.StokKodu }).IsUnique();
            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => e.Barkod);
            entity.HasIndex(e => e.Kategori);
            entity.HasIndex(e => e.StokAdi);

            entity.HasOne(e => e.Tenant)
                  .WithMany(t => t.StokKartlari)
                  .HasForeignKey(e => e.TenantId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Fatura Configuration
        modelBuilder.Entity<Fatura>(entity =>
        {
            entity.ToTable("Faturalar");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FaturaNo).HasMaxLength(50).IsRequired();
            entity.Property(e => e.AraToplam).HasColumnType("decimal(18,2)");
            entity.Property(e => e.KdvToplam).HasColumnType("decimal(18,2)");
            entity.Property(e => e.IndirimToplam).HasColumnType("decimal(18,2)");
            entity.Property(e => e.GenelToplam).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Aciklama).HasMaxLength(500);
            entity.HasIndex(e => new { e.TenantId, e.FaturaNo }).IsUnique();
            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => e.FaturaTarihi);
            entity.HasIndex(e => e.CariId);
            entity.HasIndex(e => e.Durum);

            entity.HasOne(e => e.Tenant)
                  .WithMany(t => t.Faturalar)
                  .HasForeignKey(e => e.TenantId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Cari)
                  .WithMany(c => c.Faturalar)
                  .HasForeignKey(e => e.CariId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.OlusturanKullanici)
                  .WithMany(u => u.OlusturulanFaturalar)
                  .HasForeignKey(e => e.OlusturanKullaniciId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // FaturaSatiri Configuration
        modelBuilder.Entity<FaturaSatiri>(entity =>
        {
            entity.ToTable("FaturaSatirlari");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Miktar).HasColumnType("decimal(18,3)");
            entity.Property(e => e.BirimFiyat).HasColumnType("decimal(18,2)");
            entity.Property(e => e.KdvTutar).HasColumnType("decimal(18,2)");
            entity.Property(e => e.IndirimOrani).HasColumnType("decimal(5,2)");
            entity.Property(e => e.IndirimTutar).HasColumnType("decimal(18,2)");
            entity.Property(e => e.ToplamTutar).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Aciklama).HasMaxLength(200);
            entity.HasIndex(e => e.FaturaId);
            entity.HasIndex(e => e.StokId);

            entity.HasOne(e => e.Fatura)
                  .WithMany(f => f.FaturaSatirlari)
                  .HasForeignKey(e => e.FaturaId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Stok)
                  .WithMany(s => s.FaturaSatirlari)
                  .HasForeignKey(e => e.StokId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Masa Configuration
        modelBuilder.Entity<Masa>(entity =>
        {
            entity.ToTable("Masalar");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.MasaNo).HasMaxLength(20).IsRequired();
            entity.Property(e => e.MasaAdi).HasMaxLength(50);
            entity.Property(e => e.Bolum).HasMaxLength(50);
            entity.HasIndex(e => new { e.TenantId, e.MasaNo }).IsUnique();
            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => e.Durum);

            entity.HasOne(e => e.Tenant)
                  .WithMany(t => t.Masalar)
                  .HasForeignKey(e => e.TenantId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Adisyon Configuration
        modelBuilder.Entity<Adisyon>(entity =>
        {
            entity.ToTable("Adisyonlar");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AdisyonNo).HasMaxLength(50).IsRequired();
            entity.Property(e => e.AraToplam).HasColumnType("decimal(18,2)");
            entity.Property(e => e.IndirimToplam).HasColumnType("decimal(18,2)");
            entity.Property(e => e.GenelToplam).HasColumnType("decimal(18,2)");
            entity.Property(e => e.OdenenTutar).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Aciklama).HasMaxLength(200);
            entity.HasIndex(e => new { e.TenantId, e.AdisyonNo }).IsUnique();
            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => e.MasaId);
            entity.HasIndex(e => e.Durum);
            entity.HasIndex(e => e.AcilisTarihi);

            entity.HasOne(e => e.Tenant)
                  .WithMany(t => t.Adisyonlar)
                  .HasForeignKey(e => e.TenantId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Masa)
                  .WithMany(m => m.Adisyonlar)
                  .HasForeignKey(e => e.MasaId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Garson)
                  .WithMany(u => u.Adisyonlar)
                  .HasForeignKey(e => e.GarsonId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // AdisyonSatiri Configuration
        modelBuilder.Entity<AdisyonSatiri>(entity =>
        {
            entity.ToTable("AdisyonSatirlari");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Miktar).HasColumnType("decimal(18,3)");
            entity.Property(e => e.BirimFiyat).HasColumnType("decimal(18,2)");
            entity.Property(e => e.IndirimOrani).HasColumnType("decimal(5,2)");
            entity.Property(e => e.IndirimTutar).HasColumnType("decimal(18,2)");
            entity.Property(e => e.ToplamTutar).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Not).HasColumnName("Not1").HasMaxLength(200);
            entity.HasIndex(e => e.AdisyonId);
            entity.HasIndex(e => e.StokId);

            entity.HasOne(e => e.Adisyon)
                  .WithMany(a => a.AdisyonSatirlari)
                  .HasForeignKey(e => e.AdisyonId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Stok)
                  .WithMany(s => s.AdisyonSatirlari)
                  .HasForeignKey(e => e.StokId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // CrmMusteri Configuration
        modelBuilder.Entity<CrmMusteri>(entity =>
        {
            entity.ToTable("CrmMusteriler");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.MusteriKodu).HasMaxLength(50).IsRequired();
            entity.Property(e => e.MusteriAdi).HasMaxLength(200).IsRequired();
            entity.Property(e => e.FirmaAdi).HasMaxLength(200);
            entity.Property(e => e.Telefon).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Adres).HasMaxLength(500);
            entity.Property(e => e.Il).HasMaxLength(50);
            entity.Property(e => e.Ilce).HasMaxLength(50);
            entity.Property(e => e.Sektor).HasMaxLength(100);
            entity.Property(e => e.MusteriKaynagi).HasMaxLength(100);
            entity.Property(e => e.MusteriDurumu).HasMaxLength(50);
            entity.Property(e => e.Not).HasColumnName("Not1").HasMaxLength(1000);
            entity.HasIndex(e => new { e.TenantId, e.MusteriKodu }).IsUnique();
            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => e.MusteriDurumu);
            entity.HasIndex(e => e.AtananKullaniciId);

            entity.HasOne(e => e.Tenant)
                  .WithMany(t => t.CrmMusteriler)
                  .HasForeignKey(e => e.TenantId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.AtananKullanici)
                  .WithMany(u => u.AtananMusteriler)
                  .HasForeignKey(e => e.AtananKullaniciId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // CrmAktivite Configuration
        modelBuilder.Entity<CrmAktivite>(entity =>
        {
            entity.ToTable("CrmAktiviteler");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AktiviteTipi).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Baslik).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Aciklama).HasMaxLength(1000);
            entity.Property(e => e.Durum).HasMaxLength(50);
            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => e.CrmMusteriId);
            entity.HasIndex(e => e.PlanlananTarih);
            entity.HasIndex(e => e.Durum);

            entity.HasOne(e => e.Tenant)
                  .WithMany(t => t.CrmAktiviteler)
                  .HasForeignKey(e => e.TenantId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.CrmMusteri)
                  .WithMany(c => c.Aktiviteler)
                  .HasForeignKey(e => e.CrmMusteriId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.SorumluKullanici)
                  .WithMany(u => u.SorumluAktiviteler)
                  .HasForeignKey(e => e.SorumluKullaniciId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
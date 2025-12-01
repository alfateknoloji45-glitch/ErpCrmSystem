using Microsoft.EntityFrameworkCore;
using ErpCrm.Infrastructure.Data;
using ErpCrm.Application.Services;
using ErpCrm.Core.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// DbContext registration - SQLite (no SQL Server required!)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Service registration
builder.Services.AddScoped<AuthService>();

// Controller registration
builder.Services.AddControllers();

// CORS configuration
// NOT: Production'da spesifik origin'ler belirtilmelidir
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        // Geliştirme ortamı için tüm kaynaklara izin ver
        // Production'da değiştirilmeli
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Swagger/OpenAPI setup
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "ERP/CRM/POS API",
        Version = "v1",
        Description = "Multi-tenant ERP, CRM ve POS sistemi API'si"
    });
});

var app = builder.Build();

// Auto-create SQLite database
// Note: Using EnsureCreated() for simplicity since this is a demo/development environment.
// For production, consider using dbContext.Database.Migrate() with proper migrations.
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        dbContext.Database.EnsureCreated(); // Creates DB file if not exists
        
        // Seed demo data if database is empty
        if (!dbContext.Tenants.Any())
        {
            SeedDemoData(dbContext);
        }
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while creating the database.");
    }
}

static void SeedDemoData(AppDbContext context)
{
    // FIRST: Create ALL modules if they don't exist
    if (!context.Modules.Any())
    {
        var modules = new List<Module>
        {
            new Module { ModulKodu = "CARI", ModulAdi = "Cari Yönetimi", Aciklama = "Müşteri/Tedarikçi", Kategori = "Finans", Aktif = true, OlusturmaTarihi = DateTime.Now },
            new Module { ModulKodu = "STOK", ModulAdi = "Stok Yönetimi", Aciklama = "Ürün ve Stok", Kategori = "Depo", Aktif = true, OlusturmaTarihi = DateTime.Now },
            new Module { ModulKodu = "FATURA", ModulAdi = "Fatura", Aciklama = "Alış/Satış Fatura", Kategori = "Finans", Aktif = true, OlusturmaTarihi = DateTime.Now },
            new Module { ModulKodu = "POS", ModulAdi = "POS Sistemi", Aciklama = "Masa ve Adisyon", Kategori = "Satış", Aktif = true, OlusturmaTarihi = DateTime.Now },
            new Module { ModulKodu = "CRM", ModulAdi = "CRM", Aciklama = "Müşteri İlişkileri", Kategori = "Satış", Aktif = true, OlusturmaTarihi = DateTime.Now },
            new Module { ModulKodu = "RAPORLAMA", ModulAdi = "Raporlama", Aciklama = "Raporlar", Kategori = "Analiz", Aktif = true, OlusturmaTarihi = DateTime.Now }
        };
        context.Modules.AddRange(modules);
        context.SaveChanges();
    }

    // Add demo tenant
    var tenant = new Tenant
    {
        FirmaKodu = "DEMO001",
        FirmaAdi = "Demo Firma",
        VergiNo = "1234567890",
        Telefon = "0555 555 55 55",
        Email = "info@demofirma.com",
        Adres = "Demo Adres",
        Durum = TenantDurum.Demo,
        DemoMu = true,
        DemoBitisTarihi = DateTime.Now.AddDays(30),
        OlusturmaTarihi = DateTime.Now
    };
    context.Tenants.Add(tenant);
    context.SaveChanges();

    // CRITICAL: Link ALL modules to demo tenant
    var allModules = context.Modules.ToList();
    foreach (var module in allModules)
    {
        context.TenantModules.Add(new TenantModule
        {
            TenantId = tenant.Id,
            ModuleId = module.Id,
            OlusturmaTarihi = DateTime.Now,
            Aktif = true
        });
    }
    context.SaveChanges();

    // Add demo user (password: Demo123!)
    var user = new User
    {
        TenantId = tenant.Id,
        AdSoyad = "Demo Kullanıcı",
        Email = "admin@demo.com",
        PasswordHash = AuthService.HashPassword("Demo123!"),
        Rol = UserRol.TenantAdmin,
        Aktif = true,
        OlusturmaTarihi = DateTime.Now
    };
    context.Users.Add(user);
    context.SaveChanges();

    // Add 5 demo customers (Cariler)
    var cariler = new List<Cari>
    {
        new Cari { TenantId = tenant.Id, CariKodu = "C001", CariAdi = "ABC Teknoloji A.Ş.", CariTip = CariTip.Musteri, Telefon = "0212 555 1111", Email = "info@abcteknoloji.com", Il = "İstanbul", Aktif = true, OlusturmaTarihi = DateTime.Now },
        new Cari { TenantId = tenant.Id, CariKodu = "C002", CariAdi = "XYZ Yazılım Ltd.", CariTip = CariTip.Musteri, Telefon = "0216 555 2222", Email = "info@xyzyazilim.com", Il = "İstanbul", Aktif = true, OlusturmaTarihi = DateTime.Now },
        new Cari { TenantId = tenant.Id, CariKodu = "C003", CariAdi = "Demo Müşteri 3", CariTip = CariTip.Musteri, Telefon = "0312 555 3333", Email = "musteri3@demo.com", Il = "Ankara", Aktif = true, OlusturmaTarihi = DateTime.Now },
        new Cari { TenantId = tenant.Id, CariKodu = "C004", CariAdi = "Demo Müşteri 4", CariTip = CariTip.Musteri, Telefon = "0232 555 4444", Email = "musteri4@demo.com", Il = "İzmir", Aktif = true, OlusturmaTarihi = DateTime.Now },
        new Cari { TenantId = tenant.Id, CariKodu = "T001", CariAdi = "Tedarikçi Firma Ltd.", CariTip = CariTip.Tedarikci, Telefon = "0242 555 5555", Email = "tedarik@demo.com", Il = "Antalya", Aktif = true, OlusturmaTarihi = DateTime.Now }
    };
    context.Cariler.AddRange(cariler);
    context.SaveChanges();

    // Add 10 demo products (StokKartları)
    var stoklar = new List<StokKarti>
    {
        new StokKarti { TenantId = tenant.Id, StokKodu = "S001", StokAdi = "Laptop Dell XPS 15", Birim = "Adet", Kategori = "Bilgisayar", AlisFiyati = 25000, SatisFiyati = 32000, KdvOrani = 18, StokMiktari = 10, Aktif = true, OlusturmaTarihi = DateTime.Now },
        new StokKarti { TenantId = tenant.Id, StokKodu = "S002", StokAdi = "iPhone 15 Pro", Birim = "Adet", Kategori = "Telefon", AlisFiyati = 45000, SatisFiyati = 55000, KdvOrani = 18, StokMiktari = 20, Aktif = true, OlusturmaTarihi = DateTime.Now },
        new StokKarti { TenantId = tenant.Id, StokKodu = "S003", StokAdi = "Samsung Monitor 27\"", Birim = "Adet", Kategori = "Monitör", AlisFiyati = 5000, SatisFiyati = 7000, KdvOrani = 18, StokMiktari = 15, Aktif = true, OlusturmaTarihi = DateTime.Now },
        new StokKarti { TenantId = tenant.Id, StokKodu = "S004", StokAdi = "Logitech Klavye", Birim = "Adet", Kategori = "Aksesuar", AlisFiyati = 500, SatisFiyati = 800, KdvOrani = 18, StokMiktari = 50, Aktif = true, OlusturmaTarihi = DateTime.Now },
        new StokKarti { TenantId = tenant.Id, StokKodu = "S005", StokAdi = "USB Kablo 2m", Birim = "Adet", Kategori = "Kablo", AlisFiyati = 50, SatisFiyati = 100, KdvOrani = 18, StokMiktari = 100, Aktif = true, OlusturmaTarihi = DateTime.Now },
        new StokKarti { TenantId = tenant.Id, StokKodu = "S006", StokAdi = "HP Yazıcı", Birim = "Adet", Kategori = "Yazıcı", AlisFiyati = 3000, SatisFiyati = 4500, KdvOrani = 18, StokMiktari = 8, Aktif = true, OlusturmaTarihi = DateTime.Now },
        new StokKarti { TenantId = tenant.Id, StokKodu = "S007", StokAdi = "Apple Watch", Birim = "Adet", Kategori = "Aksesuar", AlisFiyati = 12000, SatisFiyati = 15000, KdvOrani = 18, StokMiktari = 12, Aktif = true, OlusturmaTarihi = DateTime.Now },
        new StokKarti { TenantId = tenant.Id, StokKodu = "S008", StokAdi = "SSD 1TB", Birim = "Adet", Kategori = "Depolama", AlisFiyati = 2000, SatisFiyati = 3000, KdvOrani = 18, StokMiktari = 25, Aktif = true, OlusturmaTarihi = DateTime.Now },
        new StokKarti { TenantId = tenant.Id, StokKodu = "S009", StokAdi = "RAM 16GB DDR5", Birim = "Adet", Kategori = "Donanım", AlisFiyati = 1500, SatisFiyati = 2200, KdvOrani = 18, StokMiktari = 30, Aktif = true, OlusturmaTarihi = DateTime.Now },
        new StokKarti { TenantId = tenant.Id, StokKodu = "S010", StokAdi = "Mouse Pad XL", Birim = "Adet", Kategori = "Aksesuar", AlisFiyati = 100, SatisFiyati = 200, KdvOrani = 18, StokMiktari = 80, Aktif = true, OlusturmaTarihi = DateTime.Now }
    };
    context.StokKartlari.AddRange(stoklar);
    context.SaveChanges();

    // Add 3 demo invoices (Faturalar)
    var faturalar = new List<Fatura>
    {
        new Fatura { TenantId = tenant.Id, FaturaNo = "F2024-001", CariId = cariler[0].Id, OlusturanKullaniciId = user.Id, FaturaTarihi = DateTime.Now.AddDays(-5), FaturaTipi = FaturaTipi.Satis, AraToplam = 32000, KdvToplam = 5760, GenelToplam = 37760, Durum = FaturaDurum.Onaylandi, OlusturmaTarihi = DateTime.Now },
        new Fatura { TenantId = tenant.Id, FaturaNo = "F2024-002", CariId = cariler[1].Id, OlusturanKullaniciId = user.Id, FaturaTarihi = DateTime.Now.AddDays(-3), FaturaTipi = FaturaTipi.Satis, AraToplam = 55000, KdvToplam = 9900, GenelToplam = 64900, Durum = FaturaDurum.Onaylandi, OlusturmaTarihi = DateTime.Now },
        new Fatura { TenantId = tenant.Id, FaturaNo = "F2024-003", CariId = cariler[2].Id, OlusturanKullaniciId = user.Id, FaturaTarihi = DateTime.Now.AddDays(-1), FaturaTipi = FaturaTipi.Satis, AraToplam = 7000, KdvToplam = 1260, GenelToplam = 8260, Durum = FaturaDurum.Taslak, OlusturmaTarihi = DateTime.Now }
    };
    context.Faturalar.AddRange(faturalar);
    context.SaveChanges();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ERP/CRM/POS API v1");
    });
}

app.UseHttpsRedirection();

// CORS middleware
app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
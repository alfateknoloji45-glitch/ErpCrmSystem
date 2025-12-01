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

    // Add modules
    var modules = new List<Module>
    {
        new Module { ModulKodu = "CARI", ModulAdi = "Cari Yönetimi", Aciklama = "Müşteri ve tedarikçi yönetimi", Kategori = "ERP", Aktif = true, OlusturmaTarihi = DateTime.Now },
        new Module { ModulKodu = "STOK", ModulAdi = "Stok Yönetimi", Aciklama = "Ürün ve stok takibi", Kategori = "ERP", Aktif = true, OlusturmaTarihi = DateTime.Now },
        new Module { ModulKodu = "FATURA", ModulAdi = "Fatura Yönetimi", Aciklama = "Alış ve satış faturaları", Kategori = "ERP", Aktif = true, OlusturmaTarihi = DateTime.Now },
        new Module { ModulKodu = "POS", ModulAdi = "POS Sistemi", Aciklama = "Restoran/cafe satış noktası", Kategori = "POS", Aktif = true, OlusturmaTarihi = DateTime.Now },
        new Module { ModulKodu = "CRM", ModulAdi = "CRM", Aciklama = "Müşteri ilişkileri yönetimi", Kategori = "CRM", Aktif = true, OlusturmaTarihi = DateTime.Now },
        new Module { ModulKodu = "RAPORLAMA", ModulAdi = "Raporlama", Aciklama = "Raporlar ve analizler", Kategori = "GENEL", Aktif = true, OlusturmaTarihi = DateTime.Now }
    };
    context.Modules.AddRange(modules);
    context.SaveChanges();

    // Associate all modules with demo tenant
    foreach (var module in modules)
    {
        context.TenantModules.Add(new TenantModule
        {
            TenantId = tenant.Id,
            ModuleId = module.Id,
            Aktif = true,
            OlusturmaTarihi = DateTime.Now
        });
    }
    context.SaveChanges();

    // Add demo customers
    var customers = new List<Cari>
    {
        new Cari { TenantId = tenant.Id, CariKodu = "MUS001", CariAdi = "Ahmet Yılmaz", CariTip = CariTip.Musteri, Email = "ahmet@test.com", Telefon = "0555 111 22 33", Il = "İstanbul", Bakiye = 1500, OlusturmaTarihi = DateTime.Now },
        new Cari { TenantId = tenant.Id, CariKodu = "MUS002", CariAdi = "Ayşe Demir", CariTip = CariTip.Musteri, Email = "ayse@test.com", Telefon = "0555 222 33 44", Il = "Ankara", Bakiye = 2500, OlusturmaTarihi = DateTime.Now },
        new Cari { TenantId = tenant.Id, CariKodu = "MUS003", CariAdi = "Mehmet Kaya", CariTip = CariTip.Musteri, Email = "mehmet@test.com", Telefon = "0555 333 44 55", Il = "İzmir", Bakiye = 750, OlusturmaTarihi = DateTime.Now },
        new Cari { TenantId = tenant.Id, CariKodu = "TED001", CariAdi = "ABC Tedarik Ltd.", CariTip = CariTip.Tedarikci, Email = "info@abctedarik.com", Telefon = "0212 444 55 66", Il = "İstanbul", Bakiye = -3500, OlusturmaTarihi = DateTime.Now },
        new Cari { TenantId = tenant.Id, CariKodu = "MUS004", CariAdi = "Zeynep Şahin", CariTip = CariTip.Musteri, Email = "zeynep@test.com", Telefon = "0555 444 55 66", Il = "Bursa", Bakiye = 1200, OlusturmaTarihi = DateTime.Now }
    };
    context.Cariler.AddRange(customers);
    context.SaveChanges();

    // Add demo products
    var products = new List<StokKarti>
    {
        new StokKarti { TenantId = tenant.Id, StokKodu = "URN001", StokAdi = "Laptop", Kategori = "Elektronik", SatisFiyati = 15000, AlisFiyati = 12000, StokMiktari = 50, MinStokMiktari = 10, Birim = "Adet", KdvOrani = 18, OlusturmaTarihi = DateTime.Now },
        new StokKarti { TenantId = tenant.Id, StokKodu = "URN002", StokAdi = "Mouse", Kategori = "Elektronik", SatisFiyati = 150, AlisFiyati = 80, StokMiktari = 200, MinStokMiktari = 50, Birim = "Adet", KdvOrani = 18, OlusturmaTarihi = DateTime.Now },
        new StokKarti { TenantId = tenant.Id, StokKodu = "URN003", StokAdi = "Klavye", Kategori = "Elektronik", SatisFiyati = 350, AlisFiyati = 200, StokMiktari = 150, MinStokMiktari = 30, Birim = "Adet", KdvOrani = 18, OlusturmaTarihi = DateTime.Now },
        new StokKarti { TenantId = tenant.Id, StokKodu = "URN004", StokAdi = "Monitor 24\"", Kategori = "Elektronik", SatisFiyati = 3500, AlisFiyati = 2800, StokMiktari = 30, MinStokMiktari = 5, Birim = "Adet", KdvOrani = 18, OlusturmaTarihi = DateTime.Now },
        new StokKarti { TenantId = tenant.Id, StokKodu = "URN005", StokAdi = "Webcam HD", Kategori = "Elektronik", SatisFiyati = 500, AlisFiyati = 350, StokMiktari = 80, MinStokMiktari = 20, Birim = "Adet", KdvOrani = 18, OlusturmaTarihi = DateTime.Now },
        new StokKarti { TenantId = tenant.Id, StokKodu = "URN006", StokAdi = "USB Bellek 32GB", Kategori = "Aksesuar", SatisFiyati = 120, AlisFiyati = 70, StokMiktari = 300, MinStokMiktari = 100, Birim = "Adet", KdvOrani = 18, OlusturmaTarihi = DateTime.Now },
        new StokKarti { TenantId = tenant.Id, StokKodu = "URN007", StokAdi = "Harici Disk 1TB", Kategori = "Aksesuar", SatisFiyati = 1200, AlisFiyati = 900, StokMiktari = 5, MinStokMiktari = 10, Birim = "Adet", KdvOrani = 18, OlusturmaTarihi = DateTime.Now },
        new StokKarti { TenantId = tenant.Id, StokKodu = "URN008", StokAdi = "Kulaklık Bluetooth", Kategori = "Aksesuar", SatisFiyati = 800, AlisFiyati = 500, StokMiktari = 60, MinStokMiktari = 15, Birim = "Adet", KdvOrani = 18, OlusturmaTarihi = DateTime.Now },
        new StokKarti { TenantId = tenant.Id, StokKodu = "URN009", StokAdi = "Printer", Kategori = "Elektronik", SatisFiyati = 2500, AlisFiyati = 1800, StokMiktari = 15, MinStokMiktari = 5, Birim = "Adet", KdvOrani = 18, OlusturmaTarihi = DateTime.Now },
        new StokKarti { TenantId = tenant.Id, StokKodu = "URN010", StokAdi = "Tablet", Kategori = "Elektronik", SatisFiyati = 8000, AlisFiyati = 6500, StokMiktari = 25, MinStokMiktari = 5, Birim = "Adet", KdvOrani = 18, OlusturmaTarihi = DateTime.Now }
    };
    context.StokKartlari.AddRange(products);
    context.SaveChanges();

    // Add demo invoices
    var invoices = new List<Fatura>
    {
        new Fatura { TenantId = tenant.Id, FaturaNo = "FAT-2024-001", FaturaTarihi = DateTime.Now.AddDays(-5), FaturaTipi = FaturaTipi.Satis, CariId = customers[0].Id, AraToplam = 15000, KdvToplam = 2700, GenelToplam = 17700, Durum = FaturaDurum.Onaylandi, OlusturanKullaniciId = user.Id, OlusturmaTarihi = DateTime.Now.AddDays(-5) },
        new Fatura { TenantId = tenant.Id, FaturaNo = "FAT-2024-002", FaturaTarihi = DateTime.Now.AddDays(-3), FaturaTipi = FaturaTipi.Satis, CariId = customers[1].Id, AraToplam = 3850, KdvToplam = 693, GenelToplam = 4543, Durum = FaturaDurum.Onaylandi, OlusturanKullaniciId = user.Id, OlusturmaTarihi = DateTime.Now.AddDays(-3) },
        new Fatura { TenantId = tenant.Id, FaturaNo = "FAT-2024-003", FaturaTarihi = DateTime.Now.AddDays(-1), FaturaTipi = FaturaTipi.Satis, CariId = customers[2].Id, AraToplam = 8500, KdvToplam = 1530, GenelToplam = 10030, Durum = FaturaDurum.Taslak, OlusturanKullaniciId = user.Id, OlusturmaTarihi = DateTime.Now.AddDays(-1) }
    };
    context.Faturalar.AddRange(invoices);
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
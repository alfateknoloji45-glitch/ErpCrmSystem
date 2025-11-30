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

// Auto-create SQLite database and apply migrations
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
        PasswordHash = "jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=", // Demo123!
        Rol = UserRol.TenantAdmin,
        Aktif = true,
        OlusturmaTarihi = DateTime.Now
    };
    context.Users.Add(user);
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
using Microsoft.EntityFrameworkCore;
using ErpCrm.Infrastructure.Data;
using ErpCrm.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// DbContext registration
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Service registration
builder.Services.AddScoped<AuthService>();

// Controller registration
builder.Services.AddControllers();

// CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
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
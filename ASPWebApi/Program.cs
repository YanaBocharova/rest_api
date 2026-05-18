using ASPWebApi.Profiles;
using AutoMapper;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Persistence;
using Services;
using Services.Abstract.Interfaces;
using Services.AutoMaper;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Enable CORS to allow requests from any origin
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()   // Allow any domain
              .AllowAnyHeader()   // Allow any headers
              .AllowAnyMethod();  // Allow GET, POST, PUT, DELETE, etc.
    });
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PresentationApi", Version = "v1" });
});

// AutoMapper
builder.Services.AddSingleton(new MapperConfiguration(mc =>
{
    mc.AddProfile(new MappingProfile());
    mc.AddProfile(new AccountProfile());
}).CreateMapper());

// EF Core
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Services
builder.Services.AddScoped<IServiceManager, ServiceManager>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddDataProtection()
        .PersistKeysToFileSystem(new DirectoryInfo(@"/path/to/persistent/storage"))
          .ProtectKeysWithCertificate("thumbprint-of-your-certificate");

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "Google";
})
.AddCookie()
.AddGoogle(options =>
{
    options.ClientId = "YOUR_CLIENT_ID";
    options.ClientSecret = "YOUR_SECRET";
});

builder.Services.AddDataProtection()
        .PersistKeysToFileSystem(new DirectoryInfo(@"/path/to/persistent/storage"))
          .ProtectKeysWithCertificate("thumbprint-of-your-certificate");



var app = builder.Build();

// Swagger UI in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PresentationApi v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();

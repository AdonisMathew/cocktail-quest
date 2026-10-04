using System.Reflection;
using System.Text;
using CoctelIQ.Api.Configuration;
using CoctelIQ.Api.Data;
using CoctelIQ.Api.Models;
using CoctelIQ.Api.Services;
using CoctelIQ.Api.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// ---------- Base de datos ----------
// Registra el DbContext en el contenedor de Dependency Injection.
// El connection string se lee de appsettings.Development.json ("ConnectionStrings:DefaultConnection").
builder.Services.AddDbContext<CoctelIQContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ---------- Autenticación JWT ----------
var jwtSection = builder.Configuration.GetSection(JwtSettings.SectionName);
var jwtSettings = jwtSection.Get<JwtSettings>() ?? new JwtSettings();

// Fallamos al arrancar si falta configuración, en vez de emitir tokens que después no validan.
if (string.IsNullOrWhiteSpace(jwtSettings.Issuer) || string.IsNullOrWhiteSpace(jwtSettings.Audience))
{
    throw new InvalidOperationException("Faltan 'Jwt:Issuer' y/o 'Jwt:Audience' en appsettings.json.");
}

// HMAC-SHA256 necesita una clave de al menos 256 bits (32 caracteres).
if (jwtSettings.Key.Length < 32)
{
    throw new InvalidOperationException(
        "Falta 'Jwt:Key' (mínimo 32 caracteres) en la configuración. " +
        "Agregala en appsettings.Development.json; ver docs/setup-local.md.");
}

builder.Services.Configure<JwtSettings>(jwtSection);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Mantiene los nombres de los claims tal cual vienen en el token ("sub", "email"...).
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1),
            NameClaimType = JwtRegisteredClaimNames.UniqueName
        };
    });

builder.Services.AddAuthorization();

// ---------- Servicios propios ----------
builder.Services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();
builder.Services.AddSingleton<ITokenService, TokenService>();

builder.Services.AddControllers();

// ---------- Swagger ----------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CoctelIQ API",
        Version = "v1",
        Description = "API de CoctelIQ: aprendé bartending y coctelería al estilo Duolingo."
    });

    // Botón "Authorize" en Swagger UI para probar endpoints protegidos con un JWT.
    options.AddSecurityDefinition(AuthorizeOperationFilter.SchemeName, new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Pegá el token que devuelve POST /api/auth/login (sin la palabra 'Bearer')."
    });
    options.OperationFilter<AuthorizeOperationFilter>();

    // Usa los comentarios /// del código como descripciones en Swagger.
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile));
});

var app = builder.Build();

// ---------- Pipeline HTTP ----------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "CoctelIQ API v1");
        options.DocumentTitle = "CoctelIQ API";
    });

    // Solo en desarrollo: aplica las migraciones pendientes al arrancar,
    // así la base local siempre queda al día sin correr "dotnet ef database update".
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<CoctelIQContext>();
    db.Database.Migrate();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

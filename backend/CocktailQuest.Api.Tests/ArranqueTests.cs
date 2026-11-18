using System.Net;
using System.Net.Http.Headers;
using CocktailQuest.Api.Models;
using CocktailQuest.Api.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace CocktailQuest.Api.Tests;

/// <summary>
/// Pruebas de arranque: levantan la API entera en memoria y le hacen pedidos de verdad.
/// Si a Program.cs le falta registrar algo (como el AddControllers del #15),
/// estas pruebas fallan aunque el código compile.
/// </summary>
public class ArranqueTests : IClassFixture<ApiDePrueba>
{
    private readonly ApiDePrueba _api;
    private readonly HttpClient _cliente;

    public ArranqueTests(ApiDePrueba api)
    {
        _api = api;
        _cliente = api.CreateClient();
    }

    [Fact]
    public async Task Modulos_SinToken_Devuelve401()
    {
        var respuesta = await _cliente.GetAsync("/api/modulos");

        Assert.Equal(HttpStatusCode.Unauthorized, respuesta.StatusCode);
    }

    [Fact]
    public async Task Modulos_ConFirmaAdulterada_Devuelve401()
    {
        // Un token real, firmado por la API, con una letra de la firma cambiada.
        var tokens = _api.Services.GetRequiredService<ITokenService>();
        var usuario = new Usuario { Id = 1, NombreUsuario = "prueba", Email = "prueba@example.com" };
        var adulterado = CambiarPrimeraLetraDeLaFirma(tokens.GenerarToken(usuario).Token);

        using var pedido = new HttpRequestMessage(HttpMethod.Get, "/api/modulos");
        pedido.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adulterado);

        var respuesta = await _cliente.SendAsync(pedido);

        Assert.Equal(HttpStatusCode.Unauthorized, respuesta.StatusCode);
    }

    /// <summary>
    /// Un JWT tiene tres partes separadas por puntos: cabecera.datos.firma.
    /// Esto cambia la primera letra de la firma y deja el resto igual.
    /// </summary>
    private static string CambiarPrimeraLetraDeLaFirma(string token)
    {
        var partes = token.Split('.');
        var firma = partes[2];
        partes[2] = (firma[0] == 'A' ? 'B' : 'A') + firma[1..];
        return string.Join('.', partes);
    }
}

/// <summary>
/// La API en el entorno "Testing": no aplica migraciones ni carga contenido,
/// así que no necesita PostgreSQL. Los pedidos de estas pruebas se cortan en el
/// token, antes de llegar a la base.
/// </summary>
public class ApiDePrueba : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        // Program.cs no arranca sin una clave de al menos 32 caracteres.
        builder.UseSetting("Jwt:Key", "clave-solo-para-las-pruebas-0123456789");
    }
}

using System.Text.Json;
using CocktailQuest.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CocktailQuest.Api.Data;

/// <summary>
/// Carga el contenido inicial (módulos, lecciones, preguntas y opciones) desde
/// Content/contenido.json. Solo actúa si la base todavía no tiene módulos.
/// </summary>
public static class ContentSeeder
{
    private const string RutaArchivo = "Content/contenido.json";

    public static async Task SeedAsync(CocktailQuestContext db, ILogger logger)
    {
        // 1. Si ya hay contenido, no hacemos nada (así puede correr en cada arranque).
        if (await db.Modulos.AnyAsync())
        {
            return;
        }

        // 2. Leer el archivo, que se copia junto a la DLL al compilar.
        var ruta = Path.Combine(AppContext.BaseDirectory, RutaArchivo);
        await using var stream = File.OpenRead(ruta);

        // "Web" = camelCase y sin distinguir mayúsculas: "nombre" del JSON → Nombre en C#.
        var opciones = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        var archivo = await JsonSerializer.DeserializeAsync<ArchivoContenido>(stream, opciones)
            ?? throw new InvalidOperationException($"'{RutaArchivo}' está vacío.");

        // 3. Validar ANTES de tocar la base.
        Validar(archivo.Modulos);

        // 4. Insertar todo el árbol de una vez: EF completa las FK solo.
        db.Modulos.AddRange(archivo.Modulos);
        await db.SaveChangesAsync();

        var lecciones = archivo.Modulos.SelectMany(m => m.Lecciones).ToList();
        logger.LogInformation(
            "Contenido inicial cargado: {Modulos} módulo(s), {Lecciones} lecciones, {Preguntas} preguntas.",
            archivo.Modulos.Count, lecciones.Count, lecciones.Sum(l => l.Preguntas.Count));
    }

    private static void Validar(List<Modulo> modulos)
    {
        var preguntas = modulos
            .SelectMany(m => m.Lecciones)
            .SelectMany(l => l.Preguntas);

        foreach (var pregunta in preguntas)
        {
            var correctas = pregunta.Opciones.Count(o => o.EsCorrecta);
            if (pregunta.Opciones.Count < 2 || correctas != 1)
            {
                throw new InvalidOperationException(
                    $"Contenido inválido en la pregunta \"{pregunta.Enunciado}\": " +
                    $"tiene {pregunta.Opciones.Count} opciones y {correctas} correctas " +
                    "(necesita al menos 2 opciones y exactamente 1 correcta).");
            }
        }
    }

    /// <summary>Forma del archivo: { "modulos": [ ... ] }.</summary>
    private sealed class ArchivoContenido
    {
        public List<Modulo> Modulos { get; set; } = new();
    }
}
using CocktailQuest.Api.Configuration;
using CocktailQuest.Api.Models;
using CocktailQuest.Api.Services;
using Microsoft.Extensions.Options;

namespace CocktailQuest.Api.Tests;

/// <summary>
/// Pruebas de las reglas del quiz. El QuizService no usa la base, así que alcanza
/// con armar una lección en memoria: no hace falta PostgreSQL ni levantar la API.
/// </summary>
public class QuizServiceTests
{
    // El servicio real, con el mismo umbral que appsettings.json: 70.
    private readonly QuizService _servicio =
        new(Options.Create(new QuizSettings { PorcentajeAprobacion = 70 }));

    // ---------- Puntaje y aprobación ----------

    [Fact]
    public void Corregir_TodasBien_Da100YAprueba()
    {
        var leccion = CrearLeccion(cantidadPreguntas: 5);

        var resultado = _servicio.Corregir(leccion, Responder(leccion, correctas: 5));

        Assert.Equal(5, resultado.Correctas);
        Assert.Equal(100, resultado.Puntaje);
        Assert.True(resultado.Aprobado);
    }

    [Fact]
    public void Corregir_TodasMal_Da0YNoAprueba()
    {
        var leccion = CrearLeccion(cantidadPreguntas: 5);

        var resultado = _servicio.Corregir(leccion, Responder(leccion, correctas: 0));

        Assert.Equal(0, resultado.Correctas);
        Assert.Equal(0, resultado.Puntaje);
        Assert.False(resultado.Aprobado);
    }

    [Fact]
    public void Corregir_JustoEnElUmbral_Aprueba()
    {
        // 7 de 10 = 70: el umbral cuenta como aprobado (>= 70).
        var leccion = CrearLeccion(cantidadPreguntas: 10);

        var resultado = _servicio.Corregir(leccion, Responder(leccion, correctas: 7));

        Assert.Equal(70, resultado.Puntaje);
        Assert.True(resultado.Aprobado);
    }

    [Fact]
    public void Corregir_DebajoDelUmbral_NoAprueba()
    {
        // 6 de 10 = 60.
        var leccion = CrearLeccion(cantidadPreguntas: 10);

        var resultado = _servicio.Corregir(leccion, Responder(leccion, correctas: 6));

        Assert.Equal(60, resultado.Puntaje);
        Assert.False(resultado.Aprobado);
    }

    [Theory]
    [InlineData(5, 8, 63)] // 62,5: el ,5 sube
    [InlineData(1, 3, 33)] // 33,33...
    [InlineData(2, 3, 67)] // 66,66...
    [InlineData(5, 6, 83)] // 83,33...: lo mínimo para aprobar la lección 3
    public void CalcularPuntaje_RedondeaComoEnLaEscuela(int correctas, int total, int esperado)
    {
        Assert.Equal(esperado, QuizService.CalcularPuntaje(correctas, total));
    }

    [Fact]
    public void CalcularPuntaje_SinPreguntas_TiraExcepcion()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => QuizService.CalcularPuntaje(0, 0));
    }

    // ---------- Validación ----------

    [Fact]
    public void Validar_RespuestasCompletas_NoDevuelveErrores()
    {
        var leccion = CrearLeccion(cantidadPreguntas: 3);

        var errores = _servicio.Validar(leccion, Responder(leccion, correctas: 2));

        Assert.Empty(errores);
    }

    [Fact]
    public void Validar_OpcionDeOtraPregunta_DevuelveError()
    {
        var leccion = CrearLeccion(cantidadPreguntas: 3);
        var respuestas = new List<RespuestaEnviada>
        {
            new(1, Correcta(1)),
            new(2, Correcta(3)), // la opción 31 es de la pregunta 3
            new(3, Correcta(3)),
        };

        var errores = _servicio.Validar(leccion, respuestas);

        Assert.Equal("La opción 31 no es de la pregunta 2.", Assert.Single(errores));
    }

    [Fact]
    public void Validar_PreguntaRepetida_DevuelveError()
    {
        var leccion = CrearLeccion(cantidadPreguntas: 2);
        var respuestas = new List<RespuestaEnviada>
        {
            new(1, Correcta(1)),
            new(1, Incorrecta(1)), // la pregunta 1, otra vez
            new(2, Correcta(2)),
        };

        var errores = _servicio.Validar(leccion, respuestas);

        Assert.Equal("La pregunta 1 está respondida más de una vez.", Assert.Single(errores));
    }

    [Fact]
    public void Validar_PreguntasFaltantes_DevuelveError()
    {
        var leccion = CrearLeccion(cantidadPreguntas: 5);
        var soloTres = Responder(leccion, correctas: 5).Take(3).ToList(); // faltan la 4 y la 5

        var errores = _servicio.Validar(leccion, soloTres);

        Assert.Equal("Faltan responder las preguntas: 4, 5.", Assert.Single(errores));
    }

    [Fact]
    public void Validar_PreguntaDeOtraLeccion_DevuelveError()
    {
        var leccion = CrearLeccion(cantidadPreguntas: 2);
        var respuestas = new List<RespuestaEnviada>
        {
            new(1, Correcta(1)),
            new(2, Correcta(2)),
            new(99, 991), // esta lección no tiene pregunta 99
        };

        var errores = _servicio.Validar(leccion, respuestas);

        Assert.Equal("La pregunta 99 no es de esta lección.", Assert.Single(errores));
    }

    [Fact]
    public void Corregir_RespuestasInvalidas_TiraExcepcion()
    {
        // La red de seguridad: Corregir vuelve a validar aunque el controller ya lo haya hecho.
        var leccion = CrearLeccion(cantidadPreguntas: 3);
        var incompletas = Responder(leccion, correctas: 3).Take(2).ToList();

        Assert.Throws<ArgumentException>(() => _servicio.Corregir(leccion, incompletas));
    }

    // ---------- Detalle de la corrección ----------

    // Aunque el usuario responda mal, el resultado le muestra cuál era la correcta:
    // así aprende del error, como en Duolingo.
    [Fact]
    public void Corregir_RespuestaMal_MuestraLaCorrecta()
    {
        // Preparar: 3 preguntas, las 2 primeras bien y la 3 mal.
        var leccion = CrearLeccion(cantidadPreguntas: 3);
        var respuestas = Responder(leccion, correctas: 2);

        // Ejecutar
        var resultado = _servicio.Corregir(leccion, respuestas);

        // Verificar: la pregunta 3 figura mal, con la opción que eligió y la correcta.
        var pregunta3 = resultado.Detalle.Single(d => d.PreguntaId == 3);

        Assert.False(pregunta3.EsCorrecta);
        Assert.Equal(Incorrecta(3), pregunta3.OpcionElegidaId);
        Assert.Equal(Correcta(3), pregunta3.OpcionCorrectaId);
    }

    // ---------- Ayudantes ----------

    /// <summary>
    /// Una lección en memoria con las preguntas 1, 2, 3... y cuatro opciones cada una.
    /// Las opciones se numeran pregunta × 10 + n (la pregunta 3 tiene la 31, 32, 33 y 34),
    /// y la correcta es siempre la primera.
    /// </summary>
    private static Leccion CrearLeccion(int cantidadPreguntas)
    {
        var leccion = new Leccion { Id = 1, Titulo = "Lección de prueba" };

        for (var p = 1; p <= cantidadPreguntas; p++)
        {
            var pregunta = new Pregunta { Id = p, LeccionId = leccion.Id, Enunciado = $"Pregunta {p}" };

            for (var n = 1; n <= 4; n++)
            {
                pregunta.Opciones.Add(new Opcion
                {
                    Id = p * 10 + n,
                    PreguntaId = p,
                    Texto = $"Opción {n}",
                    EsCorrecta = n == 1,
                });
            }

            leccion.Preguntas.Add(pregunta);
        }

        return leccion;
    }

    /// <summary>
    /// Responde bien las primeras <paramref name="correctas"/> preguntas y mal el resto.
    /// </summary>
    private static List<RespuestaEnviada> Responder(Leccion leccion, int correctas) =>
        leccion.Preguntas
            .Select((pregunta, i) => new RespuestaEnviada(
                pregunta.Id,
                i < correctas ? Correcta(pregunta.Id) : Incorrecta(pregunta.Id)))
            .ToList();

    private static int Correcta(int preguntaId) => preguntaId * 10 + 1;

    private static int Incorrecta(int preguntaId) => preguntaId * 10 + 2;
}

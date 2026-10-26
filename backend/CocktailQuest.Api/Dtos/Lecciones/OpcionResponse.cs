using CocktailQuest.Api.Models;

namespace CocktailQuest.Api.Dtos.Lecciones;

/// <summary>
/// Una opción de respuesta. A propósito NO incluye EsCorrecta: es la respuesta del quiz.
/// </summary>
public class OpcionResponse
{
    public int Id { get; set; }
    public string Texto { get; set; } = string.Empty;

    public static OpcionResponse Desde(Opcion opcion) => new()
    {
        Id = opcion.Id,
        Texto = opcion.Texto
    };
}
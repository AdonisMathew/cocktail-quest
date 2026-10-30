using System.ComponentModel.DataAnnotations;

namespace CocktailQuest.Api.Dtos.Lecciones;

/// <summary>
/// Las respuestas del usuario al quiz de una lección: una por pregunta.
/// </summary>
public class RendirQuizRequest
{
    /// <summary>
    /// Una respuesta por cada pregunta de la lección.
    /// </summary>
    [Required(ErrorMessage = "Las respuestas son obligatorias.")]
    public List<RespuestaQuizRequest> Respuestas { get; set; } = new();
}
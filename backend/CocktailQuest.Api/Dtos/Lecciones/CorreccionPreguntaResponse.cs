using CocktailQuest.Api.Services;

namespace CocktailQuest.Api.Dtos.Lecciones;

/// <summary>
/// La corrección de una pregunta. Ahora sí se revela cuál era la opción correcta.
/// </summary>
public class CorreccionPreguntaResponse
{
    public int PreguntaId { get; set; }
    public int OpcionElegidaId { get; set; }
    public int OpcionCorrectaId { get; set; }
    public bool EsCorrecta { get; set; }

    public static CorreccionPreguntaResponse Desde(CorreccionPregunta correccion) => new()
    {
        PreguntaId = correccion.PreguntaId,
        OpcionElegidaId = correccion.OpcionElegidaId,
        OpcionCorrectaId = correccion.OpcionCorrectaId,
        EsCorrecta = correccion.EsCorrecta
    };
}
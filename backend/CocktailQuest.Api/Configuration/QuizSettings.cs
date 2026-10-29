namespace CocktailQuest.Api.Configuration;

/// <summary>
/// Configuración del quiz. Se lee de la sección "Quiz" de appsettings.
/// </summary>
public class QuizSettings
{
    public const string SectionName = "Quiz";

    /// <summary>
    /// Puntaje mínimo, de 1 a 100, para aprobar una lección.
    /// </summary>
    public int PorcentajeAprobacion { get; set; } = 70;
}
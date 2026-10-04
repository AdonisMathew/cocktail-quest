namespace CoctelIQ.Api.Configuration;

/// <summary>
/// Configuración de los tokens JWT. Se lee de la sección "Jwt" de appsettings.
/// La clave (Key) es un secreto: va en appsettings.Development.json (ignorado por Git),
/// nunca en appsettings.json.
/// </summary>
public class JwtSettings
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public int ExpiracionMinutos { get; set; } = 60;
}

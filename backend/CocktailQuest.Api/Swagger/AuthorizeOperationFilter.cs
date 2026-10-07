using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CocktailQuest.Api.Swagger;

/// <summary>
/// Marca con candado en Swagger solo los endpoints que tienen [Authorize],
/// para que Swagger UI mande el token únicamente donde hace falta.
/// </summary>
public class AuthorizeOperationFilter : IOperationFilter
{
    public const string SchemeName = "Bearer";

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var metadata = context.ApiDescription.ActionDescriptor.EndpointMetadata;
        var requiereAuth = metadata.OfType<IAuthorizeData>().Any()
            && !metadata.OfType<IAllowAnonymous>().Any();

        if (!requiereAuth)
        {
            return;
        }

        operation.Security ??= [];
        operation.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference(SchemeName, context.Document)] = []
        });
    }
}

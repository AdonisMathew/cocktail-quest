# Setup local

Cómo levantar el backend de CocktailQuest en tu máquina.

## Requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- PostgreSQL con una base llamada `cocktail_quest_db`
- Git

## 1. Clonar el repo

```bash
git clone https://github.com/AdonisMathew/cocktail-quest.git
cd cocktail-quest/backend/CocktailQuest.Api
```

## 2. Configuración local con secretos

Creá `backend/CocktailQuest.Api/appsettings.Development.json`. Este archivo está en el `.gitignore`: tiene contraseñas, **nunca se sube a GitHub**.

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=cocktail_quest_db;Username=postgres;Password=TU_PASSWORD"
  },
  "Jwt": {
    "Key": "UNA_CLAVE_LARGA_Y_ALEATORIA_DE_AL_MENOS_32_CARACTERES"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

- `DefaultConnection`: los datos de tu PostgreSQL local.
- `Jwt:Key`: la clave con la que se firman los tokens. Mínimo 32 caracteres. Si falta, la API no arranca y te lo dice.
- El resto de la configuración JWT (`Issuer`, `Audience`, `ExpiracionMinutos`) no es secreta y está en `appsettings.json`.

ASP.NET Core combina los dos archivos: primero lee `appsettings.json` y después pisa o agrega lo que haya en `appsettings.Development.json`.

## 3. Correr la API

```bash
dotnet run
```

En desarrollo, al arrancar la API aplica sola las migraciones pendientes (`Database.Migrate()` en `Program.cs`), así que las tablas se crean en el primer `dotnet run`. Después abrí Swagger en `http://localhost:5263/swagger`.

## 4. Migraciones (cuando cambie el modelo)

Cada vez que agregues o cambies una entidad, generá una migración nueva:

```bash
dotnet tool install --global dotnet-ef   # solo la primera vez
dotnet ef migrations add NombreDelCambio
```

Se crea un archivo en `Migrations/` que **sí se sube al repo**. Para aplicarla podés volver a correr la API, o usar `dotnet ef database update`.

## 5. Flujo de ramas

- `main` → siempre estable.
- `feature/nombre-feature` → una rama por feature o sprint, que se mergea a `main` con un Pull Request.

El Pull Request corre el CI (`.github/workflows/backend-ci.yml`): compila el backend y verifica que no falten migraciones.

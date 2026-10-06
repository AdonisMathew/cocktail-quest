# Diseño de base de datos

## Diagrama Entidad-Relación

```mermaid
erDiagram
    USUARIO ||--o{ PROGRESO_LECCION : tiene
    USUARIO ||--o{ RACHA : tiene
    USUARIO ||--o{ INTENTO_QUIZ : realiza
    MODULO ||--o{ LECCION : contiene
    LECCION ||--o{ PREGUNTA : contiene
    LECCION ||--o{ PROGRESO_LECCION : "es completada en"
    PREGUNTA ||--o{ OPCION : tiene
    PREGUNTA ||--o{ RESPUESTA_USUARIO : "es respondida en"
    INTENTO_QUIZ ||--o{ RESPUESTA_USUARIO : contiene
    CATEGORIA_COCKTAIL ||--o{ COCKTAIL : clasifica
    COCKTAIL ||--o{ INGREDIENTE_COCKTAIL : contiene

    USUARIO {
        int Id PK
        string NombreUsuario
        string Email
        string PasswordHash
        int Xp
        int Nivel
        datetime FechaRegistro
    }

    MODULO {
        int Id PK
        string Nombre
        string Descripcion
        int Orden
    }

    LECCION {
        int Id PK
        int ModuloId FK
        string Titulo
        string Contenido
        int Orden
        int XpRecompensa
    }

    PREGUNTA {
        int Id PK
        int LeccionId FK
        string Enunciado
        string TipoPregunta
    }

    OPCION {
        int Id PK
        int PreguntaId FK
        string Texto
        bool EsCorrecta
    }

    PROGRESO_LECCION {
        int Id PK
        int UsuarioId FK
        int LeccionId FK
        bool Completada
        datetime FechaCompletado
        int Puntaje
    }

    INTENTO_QUIZ {
        int Id PK
        int UsuarioId FK
        int LeccionId FK
        datetime Fecha
        int PuntajeTotal
    }

    RESPUESTA_USUARIO {
        int Id PK
        int IntentoQuizId FK
        int PreguntaId FK
        int OpcionSeleccionadaId FK
        bool EsCorrecta
    }

    RACHA {
        int Id PK
        int UsuarioId FK
        int DiasConsecutivos
        date UltimaFechaActividad
    }

    CATEGORIA_COCKTAIL {
        int Id PK
        string Nombre
    }

    COCKTAIL {
        int Id PK
        int CategoriaId FK
        string Nombre
        string Instrucciones
        string Dificultad
        string ImagenUrl
    }

    INGREDIENTE_COCKTAIL {
        int Id PK
        int CocktailId FK
        string Nombre
        string Cantidad
    }
```

## Notas sobre el diseño

**Usuario**
Guarda XP y Nivel desnormalizados (en vez de calcularlos siempre desde Progreso) por performance: se consultan en cada request para el ranking y el dashboard, así que conviene tenerlos ya calculados y actualizarlos cuando el usuario completa una lección.

**Lección vs Pregunta vs Opción**
Una lección tiene contenido teórico (`Contenido`, texto/markdown) y un quiz asociado. Cada pregunta tiene múltiples opciones, una sola marcada como correcta (`EsCorrecta`). Esto permite preguntas de opción múltiple; si más adelante queremos otros tipos (verdadero/falso, completar), el campo `TipoPregunta` ya deja la puerta abierta.

**Progreso vs Intento**
Separamos `ProgresoLeccion` (el estado final: ¿está completada, qué puntaje sacó la última vez?) de `IntentoQuiz` (el historial de cada vez que el usuario rindió el quiz). Esto permite mostrar "mejor puntaje" y también "historial de intentos" sin duplicar lógica.

**Racha**
Se recalcula en el backend cada vez que el usuario completa una lección: si `UltimaFechaActividad` fue ayer, se suma 1 a `DiasConsecutivos`; si fue hoy, no cambia; si fue antes de ayer, se resetea a 1.

**Cocktail**
Por ahora es de solo lectura (contenido curado por nosotros), no hay relación con Usuario todavía. Se podría agregar más adelante una tabla `Favorito` (Usuario-Cocktail) si queremos que los usuarios guarden recetas favoritas.

## Decisiones pendientes / a revisar en Sprint 1

- [ ] ¿`TipoPregunta` es un enum en C# o una tabla separada? → Por simplicidad, arrancamos con enum.
- [x] ¿Los índices en `Email` y `NombreUsuario` deben ser únicos? → Sí. Definido con Fluent API en `CocktailQuestContext.OnModelCreating` (Email máx. 256 caracteres, NombreUsuario máx. 50).
- [ ] ¿Soft delete o delete físico? → A definir cuando implementemos el CRUD de usuarios.

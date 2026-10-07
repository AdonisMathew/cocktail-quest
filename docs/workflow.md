# Cómo organizamos el trabajo

Este documento existe para que, si en algún momento arrancás una conversación nueva con Claude (porque esta se cortó, se volvió muy larga, o simplemente pasó tiempo), puedas retomar el proyecto sin perder contexto.

## Estructura del trabajo

- **1 chat ≈ 1 sprint o feature grande.** No tratamos de resolver todo el proyecto en una sola conversación — el contexto de un chat tiene límites, así que dividimos por sprint (ver tabla en el [README](../README.md)).
- **GitHub Projects** es el tablero de tareas real. `docs/` y el README son la documentación de referencia. El chat con Claude es donde se diseña y se escribe código, pero las decisiones importantes quedan por escrito acá.
- **Cada Issue de GitHub = una tarea concreta**, chica, que se puede terminar en una sesión de trabajo. Si una tarea se siente muy grande, la partimos en Issues más chicos.

## Cómo arrancar una conversación nueva con Claude

Si empezás un chat nuevo para seguir con el proyecto, contale:
1. Que estás trabajando en CocktailQuest (Claude ya tiene memoria de conversaciones anteriores sobre esto)
2. En qué sprint/Issue estás parado
3. Pegale o linkeá el archivo de `docs/` relevante si el chat lo necesita como contexto

## Convención de commits

Usamos [Conventional Commits](https://www.conventionalcommits.org/) simplificado:

- `feat: ...` → nueva funcionalidad
- `fix: ...` → corrección de bug
- `docs: ...` → cambios en documentación
- `chore: ...` → tareas de mantenimiento (configuración, dependencias)
- `refactor: ...` → cambios de código que no agregan funcionalidad

Si el commit cierra un Issue, agregar `(closes #N)` al final del mensaje.

## Estado actual

Ver la tabla de Roadmap en el [README](../README.md) y el board de GitHub Projects para el detalle de tareas en curso.

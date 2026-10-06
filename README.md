# 🍸 CocktailQuest

> Una plataforma estilo Duolingo para aprender bartending: cócteles, técnicas y teoría de coctelería, con lecciones, quizzes y gamificación.

![Status](https://img.shields.io/badge/status-en%20desarrollo-yellow)
![.NET](https://img.shields.io/badge/.NET-10-512BD4)
![License](https://img.shields.io/badge/license-MIT-blue)
![Backend CI](https://github.com/AdonisMathew/cocktail-quest/actions/workflows/backend-ci.yml/badge.svg)

## 📖 Sobre el proyecto

CocktailQuest nace de combinar dos mundos: bartending profesional y desarrollo de software. La idea es aprender (y enseñar) coctelería de forma gamificada — lecciones cortas, quizzes, recetas, y un sistema de progreso que premia la constancia (rachas, XP, niveles, ranking).

Es también un proyecto de portfolio: documentado, versionado y desarrollado siguiendo buenas prácticas de ingeniería de software (arquitectura en capas, control de versiones con ramas por feature y Pull Requests, testing, CI/CD).

## ✨ Features (MVP)

- [x] **Autenticación de usuarios** (registro, login, JWT)
- [ ] **Lecciones y módulos** organizados por categoría (espirituosos, técnicas, historia, mixología)
- [ ] **Quizzes interactivos** con corrección automática
- [ ] **Catálogo de recetas** de cócteles con ingredientes, pasos y dificultad
- [ ] **Sistema de progreso**: XP, niveles, lecciones completadas
- [ ] **Rachas** (streaks) de días consecutivos estudiando
- [ ] **Ranking social** (leaderboard) entre usuarios

## 🏗️ Stack tecnológico

| Capa | Tecnología |
|---|---|
| Backend | ASP.NET Core 10 Web API (C#) |
| ORM | Entity Framework Core |
| Base de datos | PostgreSQL |
| Autenticación | JWT (hash de contraseñas con `PasswordHasher` de ASP.NET Core Identity) |
| Frontend | *(a definir)* |
| Testing | xUnit |
| Documentación API | Swagger / OpenAPI |
| Control de versiones | Git (`main` + ramas `feature/*`) |

## 📂 Estructura del repositorio

```
cocktail-quest/
├── backend/          # API en ASP.NET Core
├── frontend/         # Cliente web
├── docs/             # Documentación técnica (arquitectura, DB, decisiones)
│   ├── api.md
│   ├── database-design.md
│   ├── setup-local.md
│   └── workflow.md
└── README.md
```

## 🗺️ Roadmap

| Sprint | Objetivo | Estado |
|---|---|---|
| 0 | Setup del repo, documentación, diseño de base de datos | ✅ Terminado |
| 1 | API base: Usuarios + Autenticación (JWT) | ✅ Terminado |
| 2 | Lecciones + Preguntas + lógica de quiz | ⬜ Pendiente |
| 3 | Catálogo de Cócteles/Recetas | ⬜ Pendiente |
| 4 | Progreso de usuario, XP y niveles | ⬜ Pendiente |
| 5 | Rachas + Ranking social | ⬜ Pendiente |
| 6 | Frontend conectado al backend | ⬜ Pendiente |
| 7 | Testing, pulido y deploy | ⬜ Pendiente |

## 🚀 Cómo correr el proyecto

Requisitos: [.NET 10 SDK](https://dotnet.microsoft.com/download) y PostgreSQL.

```bash
cd backend/CocktailQuest.Api
dotnet run
```

Antes de la primera vez hay que crear `appsettings.Development.json` con el connection string y la clave JWT: los pasos están en [`docs/setup-local.md`](./docs/setup-local.md). Con la API corriendo, Swagger queda en `http://localhost:5263/swagger`. Los endpoints están documentados en [`docs/api.md`](./docs/api.md).

## 🧠 Decisiones de diseño

Las decisiones técnicas importantes (por qué PostgreSQL y no SQL Server, por qué esta estructura de carpetas, etc.) se documentan en [`docs/`](./docs) a medida que se toman.

## 👤 Autor

Proyecto desarrollado por AdonisMathew como parte de su portfolio, combinando experiencia como bartender profesional con formación en desarrollo de software.

## 📄 Licencia

MIT — libre para usar, modificar y aprender de este código.

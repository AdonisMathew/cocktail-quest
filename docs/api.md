# API — Endpoints

Documentación de referencia de los endpoints. Con la API corriendo en desarrollo, la versión interactiva está en **Swagger**: `http://localhost:5263/swagger`.

Todos los errores siguen el formato estándar [Problem Details (RFC 9110)](https://www.rfc-editor.org/rfc/rfc9110): un JSON con `title` y `status`, y `errors` cuando falla la validación.

## Autenticación

La API usa **JWT** (JSON Web Tokens):

1. El cliente hace login y recibe un `token`.
2. En cada request a un endpoint protegido, lo manda en el header `Authorization: Bearer {token}`.
3. El token vence a los 60 minutos (`Jwt:ExpiracionMinutos`). Después hay que volver a hacer login.

En Swagger: hacé login, copiá el `token`, tocá **Authorize** y pegalo (sin la palabra `Bearer`).

---

## `POST /api/auth/registro`

Crea un usuario nuevo. Público.

**Body**

```json
{
  "nombreUsuario": "bartender_tucu",
  "email": "matias@example.com",
  "password": "Negroni2026!"
}
```

| Campo | Reglas |
|---|---|
| `nombreUsuario` | 3 a 50 caracteres; letras, números, `_` y `.`. Único (sin distinguir mayúsculas). |
| `email` | Formato válido. Único. Se guarda en minúsculas. |
| `password` | 8 a 100 caracteres. Se guarda hasheada (PBKDF2), nunca en texto plano. |

**Respuestas**

| Código | Cuándo |
|---|---|
| `201 Created` | Usuario creado. Devuelve el usuario (sin la contraseña). |
| `400 Bad Request` | Algún campo no pasa la validación. |
| `409 Conflict` | El email o el nombre de usuario ya están en uso. |

```json
{
  "id": 1,
  "nombreUsuario": "bartender_tucu",
  "email": "matias@example.com",
  "xp": 0,
  "nivel": 1,
  "fechaRegistro": "2026-10-04T20:29:36.033878Z"
}
```

---

## `POST /api/auth/login`

Valida las credenciales y devuelve un token. Público.

**Body**

```json
{
  "email": "matias@example.com",
  "password": "Negroni2026!"
}
```

**Respuestas**

| Código | Cuándo |
|---|---|
| `200 OK` | Credenciales correctas. |
| `400 Bad Request` | Falta algún campo o el email no tiene formato válido. |
| `401 Unauthorized` | Email o contraseña incorrectos. Es el mismo mensaje en los dos casos, para no revelar qué emails están registrados. |

```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiraEn": "2026-10-04T21:29:36Z",
  "usuario": { "id": 1, "nombreUsuario": "bartender_tucu", "email": "matias@example.com", "xp": 0, "nivel": 1, "fechaRegistro": "2026-10-04T20:29:36.033878Z" }
}
```

**Qué lleva el token** (se puede leer en [jwt.io](https://jwt.io); está firmado, no cifrado, así que no lleva datos sensibles):

| Claim | Valor |
|---|---|
| `sub` | Id del usuario |
| `unique_name` | Nombre de usuario |
| `email` | Email |
| `jti` | Id único del token |
| `iss` / `aud` | `CocktailQuest.Api` / `CocktailQuest.Client` |
| `exp` | Vencimiento |

---

## `GET /api/usuarios/me`

Devuelve el perfil del usuario dueño del token. **Requiere token.**

| Código | Cuándo |
|---|---|
| `200 OK` | Devuelve el usuario (mismo formato que el registro). |
| `401 Unauthorized` | Falta el token, está vencido o fue modificado. |
| `404 Not Found` | El usuario del token ya no existe. |

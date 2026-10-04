using CoctelIQ.Api.Data;
using CoctelIQ.Api.Dtos.Auth;
using CoctelIQ.Api.Dtos.Usuarios;
using CoctelIQ.Api.Models;
using CoctelIQ.Api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace CoctelIQ.Api.Controllers;

/// <summary>
/// Registro e inicio de sesión.
/// </summary>
[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly CoctelIQContext _context;
    private readonly IPasswordHasher<Usuario> _passwordHasher;
    private readonly ITokenService _tokenService;

    public AuthController(
        CoctelIQContext context,
        IPasswordHasher<Usuario> passwordHasher,
        ITokenService tokenService)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    /// <summary>
    /// Registra un usuario nuevo.
    /// </summary>
    /// <response code="201">Usuario creado. Devuelve sus datos públicos.</response>
    /// <response code="400">Algún dato no pasó la validación.</response>
    /// <response code="409">El email o el nombre de usuario ya están en uso.</response>
    [HttpPost("registro")]
    [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UsuarioResponse>> Registrar(RegistroRequest request)
    {
        // Normalizamos para que "Matias@Mail.com" y "matias@mail.com" sean el mismo usuario.
        var email = request.Email.Trim().ToLowerInvariant();
        var nombreUsuario = request.NombreUsuario.Trim();

        if (await _context.Usuarios.AnyAsync(u => u.Email == email))
        {
            return Problem(statusCode: StatusCodes.Status409Conflict, title: "El email ya está registrado.");
        }

        if (await _context.Usuarios.AnyAsync(u => u.NombreUsuario.ToLower() == nombreUsuario.ToLower()))
        {
            return Problem(statusCode: StatusCodes.Status409Conflict, title: "El nombre de usuario ya está en uso.");
        }

        var usuario = new Usuario
        {
            NombreUsuario = nombreUsuario,
            Email = email,
            FechaRegistro = DateTime.UtcNow
        };
        // Nunca guardamos la contraseña: guardamos un hash (PBKDF2 con salt aleatorio).
        usuario.PasswordHash = _passwordHasher.HashPassword(usuario, request.Password);

        _context.Usuarios.Add(usuario);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            // Caso borde: dos registros simultáneos con el mismo email pasan los chequeos de arriba,
            // pero el índice único de la base frena al segundo.
            return Problem(statusCode: StatusCodes.Status409Conflict, title: "El email o el nombre de usuario ya están en uso.");
        }

        return StatusCode(StatusCodes.Status201Created, UsuarioResponse.Desde(usuario));
    }

    /// <summary>
    /// Inicia sesión y devuelve un token JWT.
    /// </summary>
    /// <response code="200">Credenciales correctas. Devuelve el token y los datos del usuario.</response>
    /// <response code="400">Algún dato no pasó la validación.</response>
    /// <response code="401">Email o contraseña incorrectos.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var usuario = await _context.Usuarios.SingleOrDefaultAsync(u => u.Email == email);

        // Mismo mensaje si el email no existe o si la contraseña está mal:
        // así no le confirmamos a un atacante qué emails están registrados.
        if (usuario is null)
        {
            return CredencialesInvalidas();
        }

        var resultado = _passwordHasher.VerifyHashedPassword(usuario, usuario.PasswordHash, request.Password);
        if (resultado == PasswordVerificationResult.Failed)
        {
            return CredencialesInvalidas();
        }

        if (resultado == PasswordVerificationResult.SuccessRehashNeeded)
        {
            // El hash se generó con parámetros viejos: lo actualizamos aprovechando que tenemos la contraseña.
            usuario.PasswordHash = _passwordHasher.HashPassword(usuario, request.Password);
            await _context.SaveChangesAsync();
        }

        var token = _tokenService.GenerarToken(usuario);

        return Ok(new LoginResponse
        {
            Token = token.Token,
            ExpiraEn = token.ExpiraEn,
            Usuario = UsuarioResponse.Desde(usuario)
        });
    }

    private ObjectResult CredencialesInvalidas() =>
        Problem(statusCode: StatusCodes.Status401Unauthorized, title: "Email o contraseña incorrectos.");
}

using CocktailQuest.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CocktailQuest.Api.Data;

public class CocktailQuestContext : DbContext
{
    public CocktailQuestContext(DbContextOptions<CocktailQuestContext> options)
        : base(options)
    {
    }

    // Cada DbSet es una tabla; el nombre de la propiedad es el nombre de la tabla.
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Modulo> Modulos => Set<Modulo>();
    public DbSet<Leccion> Lecciones => Set<Leccion>();
    public DbSet<Pregunta> Preguntas => Set<Pregunta>();
    public DbSet<Opcion> Opciones => Set<Opcion>();
    public DbSet<IntentoQuiz> IntentosQuiz => Set<IntentoQuiz>();
    public DbSet<RespuestaUsuario> RespuestasUsuario => Set<RespuestaUsuario>();
    public DbSet<ProgresoLeccion> ProgresosLeccion => Set<ProgresoLeccion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Encuentra todas las clases IEntityTypeConfiguration<T> del proyecto y las aplica.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CocktailQuestContext).Assembly);
    }
}
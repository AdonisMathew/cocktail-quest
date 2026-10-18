using CocktailQuest.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CocktailQuest.Api.Data.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> entity)
    {
        entity.HasIndex(u => u.Email).IsUnique();
        entity.HasIndex(u => u.NombreUsuario).IsUnique();
        entity.Property(u => u.Email).IsRequired().HasMaxLength(256);
        entity.Property(u => u.NombreUsuario).IsRequired().HasMaxLength(50);
        entity.Property(u => u.PasswordHash).IsRequired();
    }
}
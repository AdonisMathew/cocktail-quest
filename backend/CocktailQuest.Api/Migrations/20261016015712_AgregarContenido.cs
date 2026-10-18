using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CocktailQuest.Api.Migrations
{
    /// <inheritdoc />
    public partial class AgregarContenido : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Modulos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Orden = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modulos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Lecciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ModuloId = table.Column<int>(type: "integer", nullable: false),
                    Titulo = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Contenido = table.Column<string>(type: "text", nullable: false),
                    Orden = table.Column<int>(type: "integer", nullable: false),
                    XpRecompensa = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lecciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lecciones_Modulos_ModuloId",
                        column: x => x.ModuloId,
                        principalTable: "Modulos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Preguntas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LeccionId = table.Column<int>(type: "integer", nullable: false),
                    Enunciado = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    TipoPregunta = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Preguntas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Preguntas_Lecciones_LeccionId",
                        column: x => x.LeccionId,
                        principalTable: "Lecciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Opciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PreguntaId = table.Column<int>(type: "integer", nullable: false),
                    Texto = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    EsCorrecta = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Opciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Opciones_Preguntas_PreguntaId",
                        column: x => x.PreguntaId,
                        principalTable: "Preguntas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Lecciones_ModuloId_Orden",
                table: "Lecciones",
                columns: new[] { "ModuloId", "Orden" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Modulos_Orden",
                table: "Modulos",
                column: "Orden",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Opciones_PreguntaId",
                table: "Opciones",
                column: "PreguntaId");

            migrationBuilder.CreateIndex(
                name: "IX_Preguntas_LeccionId",
                table: "Preguntas",
                column: "LeccionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Opciones");

            migrationBuilder.DropTable(
                name: "Preguntas");

            migrationBuilder.DropTable(
                name: "Lecciones");

            migrationBuilder.DropTable(
                name: "Modulos");
        }
    }
}

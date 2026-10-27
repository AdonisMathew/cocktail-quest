using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CocktailQuest.Api.Migrations
{
    /// <inheritdoc />
    public partial class AgregarIntentosYProgreso : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IntentosQuiz",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    LeccionId = table.Column<int>(type: "integer", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Puntaje = table.Column<int>(type: "integer", nullable: false),
                    Aprobado = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntentosQuiz", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IntentosQuiz_Lecciones_LeccionId",
                        column: x => x.LeccionId,
                        principalTable: "Lecciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IntentosQuiz_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProgresosLeccion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    LeccionId = table.Column<int>(type: "integer", nullable: false),
                    Completada = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCompletado = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MejorPuntaje = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgresosLeccion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProgresosLeccion_Lecciones_LeccionId",
                        column: x => x.LeccionId,
                        principalTable: "Lecciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProgresosLeccion_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RespuestasUsuario",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IntentoQuizId = table.Column<int>(type: "integer", nullable: false),
                    PreguntaId = table.Column<int>(type: "integer", nullable: false),
                    OpcionSeleccionadaId = table.Column<int>(type: "integer", nullable: false),
                    EsCorrecta = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RespuestasUsuario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RespuestasUsuario_IntentosQuiz_IntentoQuizId",
                        column: x => x.IntentoQuizId,
                        principalTable: "IntentosQuiz",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RespuestasUsuario_Opciones_OpcionSeleccionadaId",
                        column: x => x.OpcionSeleccionadaId,
                        principalTable: "Opciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RespuestasUsuario_Preguntas_PreguntaId",
                        column: x => x.PreguntaId,
                        principalTable: "Preguntas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IntentosQuiz_LeccionId",
                table: "IntentosQuiz",
                column: "LeccionId");

            migrationBuilder.CreateIndex(
                name: "IX_IntentosQuiz_UsuarioId",
                table: "IntentosQuiz",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgresosLeccion_LeccionId",
                table: "ProgresosLeccion",
                column: "LeccionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgresosLeccion_UsuarioId_LeccionId",
                table: "ProgresosLeccion",
                columns: new[] { "UsuarioId", "LeccionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RespuestasUsuario_IntentoQuizId_PreguntaId",
                table: "RespuestasUsuario",
                columns: new[] { "IntentoQuizId", "PreguntaId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RespuestasUsuario_OpcionSeleccionadaId",
                table: "RespuestasUsuario",
                column: "OpcionSeleccionadaId");

            migrationBuilder.CreateIndex(
                name: "IX_RespuestasUsuario_PreguntaId",
                table: "RespuestasUsuario",
                column: "PreguntaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProgresosLeccion");

            migrationBuilder.DropTable(
                name: "RespuestasUsuario");

            migrationBuilder.DropTable(
                name: "IntentosQuiz");
        }
    }
}

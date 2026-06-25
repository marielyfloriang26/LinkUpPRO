using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinkUpPro.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Inicializacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "VARCHAR(50)", nullable: false),
                    Apellido = table.Column<string>(type: "VARCHAR(50)", nullable: false),
                    Correo = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    NombreUsuario = table.Column<string>(type: "VARCHAR(30)", nullable: false),
                    PasswordHash = table.Column<string>(type: "VARCHAR(255)", nullable: false),
                    FotoPerfilUrl = table.Column<string>(type: "VARCHAR(255)", nullable: true),
                    EsActivo = table.Column<bool>(type: "BIT", nullable: false, defaultValue: false),
                    CodigoVerificacion = table.Column<string>(type: "VARCHAR(100)", nullable: true),
                    IntentosFallidos = table.Column<int>(type: "INT", nullable: false, defaultValue: 0),
                    BloqueoHasta = table.Column<DateTime>(type: "DATETIME", nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "DATETIME", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Amistades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId1 = table.Column<int>(type: "int", nullable: false),
                    UsuarioId2 = table.Column<int>(type: "int", nullable: false),
                    FechaAmistad = table.Column<DateTime>(type: "DATETIME", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Amistades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Amistades_Usuarios_UsuarioId1",
                        column: x => x.UsuarioId1,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Amistades_Usuarios_UsuarioId2",
                        column: x => x.UsuarioId2,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Notificaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    Mensaje = table.Column<string>(type: "VARCHAR(255)", nullable: false),
                    Leida = table.Column<bool>(type: "BIT", nullable: false, defaultValue: false),
                    FechaNotificacion = table.Column<DateTime>(type: "DATETIME", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notificaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notificaciones_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PartidasBattleship",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Jugador1Id = table.Column<int>(type: "int", nullable: false),
                    Jugador2Id = table.Column<int>(type: "int", nullable: false),
                    TurnoUsuarioId = table.Column<int>(type: "int", nullable: false),
                    EstadoPartida = table.Column<string>(type: "VARCHAR(20)", nullable: false, defaultValue: "Configurando"),
                    GanadorId = table.Column<int>(type: "int", nullable: true),
                    UltimoMovimiento = table.Column<DateTime>(type: "DATETIME", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    FechaInicio = table.Column<DateTime>(type: "DATETIME", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartidasBattleship", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PartidasBattleship_Usuarios_GanadorId",
                        column: x => x.GanadorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PartidasBattleship_Usuarios_Jugador1Id",
                        column: x => x.Jugador1Id,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PartidasBattleship_Usuarios_Jugador2Id",
                        column: x => x.Jugador2Id,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PartidasBattleship_Usuarios_TurnoUsuarioId",
                        column: x => x.TurnoUsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Publicaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    ContenidoTexto = table.Column<string>(type: "TEXT", nullable: false),
                    ImagenUrl = table.Column<string>(type: "VARCHAR(255)", nullable: true),
                    YouTubeVideoUrl = table.Column<string>(type: "VARCHAR(255)", nullable: true),
                    Privacidad = table.Column<string>(type: "VARCHAR(15)", nullable: false, defaultValue: "Publico"),
                    FechaCreacion = table.Column<DateTime>(type: "DATETIME", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Publicaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Publicaciones_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SolicitudesAmistad",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmisorId = table.Column<int>(type: "int", nullable: false),
                    ReceptorId = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<string>(type: "VARCHAR(15)", nullable: false, defaultValue: "Pendiente"),
                    FechaEnvio = table.Column<DateTime>(type: "DATETIME", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudesAmistad", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolicitudesAmistad_Usuarios_EmisorId",
                        column: x => x.EmisorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitudesAmistad_Usuarios_ReceptorId",
                        column: x => x.ReceptorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CasillasTablero",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PartidaId = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    Fila = table.Column<int>(type: "INT", nullable: false),
                    Columna = table.Column<int>(type: "INT", nullable: false),
                    TieneBarco = table.Column<bool>(type: "BIT", nullable: false, defaultValue: false),
                    Impactada = table.Column<bool>(type: "BIT", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CasillasTablero", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CasillasTablero_PartidasBattleship_PartidaId",
                        column: x => x.PartidaId,
                        principalTable: "PartidasBattleship",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CasillasTablero_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Comentarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PublicacionId = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    Contenido = table.Column<string>(type: "TEXT", nullable: false),
                    ComentarioPadreId = table.Column<int>(type: "int", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "DATETIME", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comentarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comentarios_Comentarios_ComentarioPadreId",
                        column: x => x.ComentarioPadreId,
                        principalTable: "Comentarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comentarios_Publicaciones_PublicacionId",
                        column: x => x.PublicacionId,
                        principalTable: "Publicaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comentarios_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Reacciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    PublicacionId = table.Column<int>(type: "int", nullable: true),
                    ComentarioId = table.Column<int>(type: "int", nullable: true),
                    TipoReaccion = table.Column<string>(type: "VARCHAR(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reacciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reacciones_Comentarios_ComentarioId",
                        column: x => x.ComentarioId,
                        principalTable: "Comentarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reacciones_Publicaciones_PublicacionId",
                        column: x => x.PublicacionId,
                        principalTable: "Publicaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reacciones_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Amistades_UsuarioId1",
                table: "Amistades",
                column: "UsuarioId1");

            migrationBuilder.CreateIndex(
                name: "IX_Amistades_UsuarioId2",
                table: "Amistades",
                column: "UsuarioId2");

            migrationBuilder.CreateIndex(
                name: "IX_CasillasTablero_PartidaId",
                table: "CasillasTablero",
                column: "PartidaId");

            migrationBuilder.CreateIndex(
                name: "IX_CasillasTablero_UsuarioId",
                table: "CasillasTablero",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Comentarios_ComentarioPadreId",
                table: "Comentarios",
                column: "ComentarioPadreId");

            migrationBuilder.CreateIndex(
                name: "IX_Comentarios_PublicacionId",
                table: "Comentarios",
                column: "PublicacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Comentarios_UsuarioId",
                table: "Comentarios",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Notificaciones_UsuarioId",
                table: "Notificaciones",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_PartidasBattleship_GanadorId",
                table: "PartidasBattleship",
                column: "GanadorId");

            migrationBuilder.CreateIndex(
                name: "IX_PartidasBattleship_Jugador1Id",
                table: "PartidasBattleship",
                column: "Jugador1Id");

            migrationBuilder.CreateIndex(
                name: "IX_PartidasBattleship_Jugador2Id",
                table: "PartidasBattleship",
                column: "Jugador2Id");

            migrationBuilder.CreateIndex(
                name: "IX_PartidasBattleship_TurnoUsuarioId",
                table: "PartidasBattleship",
                column: "TurnoUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Publicaciones_UsuarioId",
                table: "Publicaciones",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Reacciones_ComentarioId",
                table: "Reacciones",
                column: "ComentarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Reacciones_PublicacionId",
                table: "Reacciones",
                column: "PublicacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Reacciones_UsuarioId",
                table: "Reacciones",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesAmistad_EmisorId",
                table: "SolicitudesAmistad",
                column: "EmisorId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesAmistad_ReceptorId",
                table: "SolicitudesAmistad",
                column: "ReceptorId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Correo",
                table: "Usuarios",
                column: "Correo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_NombreUsuario",
                table: "Usuarios",
                column: "NombreUsuario",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Amistades");

            migrationBuilder.DropTable(
                name: "CasillasTablero");

            migrationBuilder.DropTable(
                name: "Notificaciones");

            migrationBuilder.DropTable(
                name: "Reacciones");

            migrationBuilder.DropTable(
                name: "SolicitudesAmistad");

            migrationBuilder.DropTable(
                name: "PartidasBattleship");

            migrationBuilder.DropTable(
                name: "Comentarios");

            migrationBuilder.DropTable(
                name: "Publicaciones");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinkUpPro.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MergeReconciliation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Amistades_AspNetUsers_UsuarioId1",
                table: "Amistades");

            migrationBuilder.DropForeignKey(
                name: "FK_Amistades_AspNetUsers_UsuarioId2",
                table: "Amistades");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_CasillasTablero_AspNetUsers_UsuarioId",
                table: "CasillasTablero");

            migrationBuilder.DropForeignKey(
                name: "FK_Comentarios_AspNetUsers_UsuarioId",
                table: "Comentarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Notificaciones_AspNetUsers_UsuarioId",
                table: "Notificaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_PartidasBattleship_AspNetUsers_GanadorId",
                table: "PartidasBattleship");

            migrationBuilder.DropForeignKey(
                name: "FK_PartidasBattleship_AspNetUsers_Jugador1Id",
                table: "PartidasBattleship");

            migrationBuilder.DropForeignKey(
                name: "FK_PartidasBattleship_AspNetUsers_Jugador2Id",
                table: "PartidasBattleship");

            migrationBuilder.DropForeignKey(
                name: "FK_PartidasBattleship_AspNetUsers_TurnoUsuarioId",
                table: "PartidasBattleship");

            migrationBuilder.DropForeignKey(
                name: "FK_Publicaciones_AspNetUsers_UsuarioId",
                table: "Publicaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_Reacciones_AspNetUsers_UsuarioId",
                table: "Reacciones");

            migrationBuilder.DropForeignKey(
                name: "FK_SolicitudesAmistad_AspNetUsers_EmisorId",
                table: "SolicitudesAmistad");

            migrationBuilder.DropForeignKey(
                name: "FK_SolicitudesAmistad_AspNetUsers_ReceptorId",
                table: "SolicitudesAmistad");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUsers",
                table: "AspNetUsers");

            migrationBuilder.RenameTable(
                name: "AspNetUsers",
                newName: "Usuarios");

            migrationBuilder.AddColumn<DateTime>(
                name: "UltimoReenvioCorreo",
                table: "Usuarios",
                type: "DATETIME",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Usuarios",
                table: "Usuarios",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Amistades_Usuarios_UsuarioId1",
                table: "Amistades",
                column: "UsuarioId1",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Amistades_Usuarios_UsuarioId2",
                table: "Amistades",
                column: "UsuarioId2",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_Usuarios_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_Usuarios_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_Usuarios_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_Usuarios_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CasillasTablero_Usuarios_UsuarioId",
                table: "CasillasTablero",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comentarios_Usuarios_UsuarioId",
                table: "Comentarios",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Notificaciones_Usuarios_UsuarioId",
                table: "Notificaciones",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PartidasBattleship_Usuarios_GanadorId",
                table: "PartidasBattleship",
                column: "GanadorId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PartidasBattleship_Usuarios_Jugador1Id",
                table: "PartidasBattleship",
                column: "Jugador1Id",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PartidasBattleship_Usuarios_Jugador2Id",
                table: "PartidasBattleship",
                column: "Jugador2Id",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PartidasBattleship_Usuarios_TurnoUsuarioId",
                table: "PartidasBattleship",
                column: "TurnoUsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Publicaciones_Usuarios_UsuarioId",
                table: "Publicaciones",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reacciones_Usuarios_UsuarioId",
                table: "Reacciones",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SolicitudesAmistad_Usuarios_EmisorId",
                table: "SolicitudesAmistad",
                column: "EmisorId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SolicitudesAmistad_Usuarios_ReceptorId",
                table: "SolicitudesAmistad",
                column: "ReceptorId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Amistades_Usuarios_UsuarioId1",
                table: "Amistades");

            migrationBuilder.DropForeignKey(
                name: "FK_Amistades_Usuarios_UsuarioId2",
                table: "Amistades");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserClaims_Usuarios_UserId",
                table: "AspNetUserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserLogins_Usuarios_UserId",
                table: "AspNetUserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_Usuarios_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_Usuarios_UserId",
                table: "AspNetUserTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_CasillasTablero_Usuarios_UsuarioId",
                table: "CasillasTablero");

            migrationBuilder.DropForeignKey(
                name: "FK_Comentarios_Usuarios_UsuarioId",
                table: "Comentarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Notificaciones_Usuarios_UsuarioId",
                table: "Notificaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_PartidasBattleship_Usuarios_GanadorId",
                table: "PartidasBattleship");

            migrationBuilder.DropForeignKey(
                name: "FK_PartidasBattleship_Usuarios_Jugador1Id",
                table: "PartidasBattleship");

            migrationBuilder.DropForeignKey(
                name: "FK_PartidasBattleship_Usuarios_Jugador2Id",
                table: "PartidasBattleship");

            migrationBuilder.DropForeignKey(
                name: "FK_PartidasBattleship_Usuarios_TurnoUsuarioId",
                table: "PartidasBattleship");

            migrationBuilder.DropForeignKey(
                name: "FK_Publicaciones_Usuarios_UsuarioId",
                table: "Publicaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_Reacciones_Usuarios_UsuarioId",
                table: "Reacciones");

            migrationBuilder.DropForeignKey(
                name: "FK_SolicitudesAmistad_Usuarios_EmisorId",
                table: "SolicitudesAmistad");

            migrationBuilder.DropForeignKey(
                name: "FK_SolicitudesAmistad_Usuarios_ReceptorId",
                table: "SolicitudesAmistad");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Usuarios",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "UltimoReenvioCorreo",
                table: "Usuarios");

            migrationBuilder.RenameTable(
                name: "Usuarios",
                newName: "AspNetUsers");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUsers",
                table: "AspNetUsers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Amistades_AspNetUsers_UsuarioId1",
                table: "Amistades",
                column: "UsuarioId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Amistades_AspNetUsers_UsuarioId2",
                table: "Amistades",
                column: "UsuarioId2",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CasillasTablero_AspNetUsers_UsuarioId",
                table: "CasillasTablero",
                column: "UsuarioId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comentarios_AspNetUsers_UsuarioId",
                table: "Comentarios",
                column: "UsuarioId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Notificaciones_AspNetUsers_UsuarioId",
                table: "Notificaciones",
                column: "UsuarioId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PartidasBattleship_AspNetUsers_GanadorId",
                table: "PartidasBattleship",
                column: "GanadorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PartidasBattleship_AspNetUsers_Jugador1Id",
                table: "PartidasBattleship",
                column: "Jugador1Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PartidasBattleship_AspNetUsers_Jugador2Id",
                table: "PartidasBattleship",
                column: "Jugador2Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PartidasBattleship_AspNetUsers_TurnoUsuarioId",
                table: "PartidasBattleship",
                column: "TurnoUsuarioId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Publicaciones_AspNetUsers_UsuarioId",
                table: "Publicaciones",
                column: "UsuarioId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reacciones_AspNetUsers_UsuarioId",
                table: "Reacciones",
                column: "UsuarioId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SolicitudesAmistad_AspNetUsers_EmisorId",
                table: "SolicitudesAmistad",
                column: "EmisorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SolicitudesAmistad_AspNetUsers_ReceptorId",
                table: "SolicitudesAmistad",
                column: "ReceptorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

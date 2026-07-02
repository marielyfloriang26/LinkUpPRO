using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinkUpPro.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificacionesCampos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Crear las nuevas columnas en la tabla Notificaciones
            migrationBuilder.AddColumn<int>(
                name: "RemitenteId",
                table: "Notificaciones",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PublicacionId",
                table: "Notificaciones",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipoActividad",
                table: "Notificaciones",
                type: "VARCHAR(50)",
                nullable: false,
                defaultValue: "");

            
            migrationBuilder.CreateIndex(
                name: "IX_Notificaciones_PublicacionId",
                table: "Notificaciones",
                column: "PublicacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Notificaciones_RemitenteId",
                table: "Notificaciones",
                column: "RemitenteId");

           
            migrationBuilder.AddForeignKey(
                name: "FK_Notificaciones_Publicaciones_PublicacionId",
                table: "Notificaciones",
                column: "PublicacionId",
                principalTable: "Publicaciones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Notificaciones_Usuarios_RemitenteId",
                table: "Notificaciones",
                column: "RemitenteId",
                principalTable: "Usuarios", 
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notificaciones_Publicaciones_PublicacionId",
                table: "Notificaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_Notificaciones_Usuarios_RemitenteId",
                table: "Notificaciones");

            migrationBuilder.DropIndex(
                name: "IX_Notificaciones_PublicacionId",
                table: "Notificaciones");

            migrationBuilder.DropIndex(
                name: "IX_Notificaciones_RemitenteId",
                table: "Notificaciones");

            migrationBuilder.DropColumn(
                name: "RemitenteId",
                table: "Notificaciones");

            migrationBuilder.DropColumn(
                name: "PublicacionId",
                table: "Notificaciones");

            migrationBuilder.DropColumn(
                name: "TipoActividad",
                table: "Notificaciones");
        }
    }
}
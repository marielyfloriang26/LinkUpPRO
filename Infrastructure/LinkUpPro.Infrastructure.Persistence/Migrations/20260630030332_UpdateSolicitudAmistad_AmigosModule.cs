using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinkUpPro.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSolicitudAmistad_AmigosModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "SolicitudesAmistad",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "En espera de respuesta",
                oldClrType: typeof(string),
                oldType: "VARCHAR(15)",
                oldDefaultValue: "Pendiente");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaRespuesta",
                table: "SolicitudesAmistad",
                type: "DATETIME",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "OcultaParaEmisor",
                table: "SolicitudesAmistad",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaRespuesta",
                table: "SolicitudesAmistad");

            migrationBuilder.DropColumn(
                name: "OcultaParaEmisor",
                table: "SolicitudesAmistad");

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "SolicitudesAmistad",
                type: "VARCHAR(15)",
                nullable: false,
                defaultValue: "Pendiente",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldDefaultValue: "En espera de respuesta");
        }
    }
}

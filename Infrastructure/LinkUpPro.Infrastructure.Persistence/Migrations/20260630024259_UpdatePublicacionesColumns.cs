using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinkUpPro.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePublicacionesColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "Amistades",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Activa");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Amistades");
        }
    }
}

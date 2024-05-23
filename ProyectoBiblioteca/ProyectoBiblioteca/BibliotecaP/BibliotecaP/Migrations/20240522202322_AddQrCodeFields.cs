using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BibliotecaP.Migrations
{
    /// <inheritdoc />
    public partial class AddQrCodeFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccesoQrBase64",
                table: "ReservacionCubiculos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SalidaQrBase64",
                table: "ReservacionCubiculos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccesoQrBase64",
                table: "ReservacionCubiculos");

            migrationBuilder.DropColumn(
                name: "SalidaQrBase64",
                table: "ReservacionCubiculos");
        }
    }
}

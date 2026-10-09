using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GlassFlowAyF.Migrations
{
    /// <inheritdoc />
    public partial class AgregarMedidasYUbicacionSolicitud : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UbicacionInstalacion",
                table: "SolicitudesCotizacion",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "UbicacionPersonalizada",
                table: "SolicitudesCotizacion",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "UnidadMedida",
                table: "SolicitudesCotizacion",
                type: "varchar(2)",
                maxLength: 2,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UbicacionInstalacion",
                table: "SolicitudesCotizacion");

            migrationBuilder.DropColumn(
                name: "UbicacionPersonalizada",
                table: "SolicitudesCotizacion");

            migrationBuilder.DropColumn(
                name: "UnidadMedida",
                table: "SolicitudesCotizacion");
        }
    }
}

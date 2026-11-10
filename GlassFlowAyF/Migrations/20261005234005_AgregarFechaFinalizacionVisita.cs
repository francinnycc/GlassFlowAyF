using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GlassFlowAyF.Migrations
{
    /// <inheritdoc />
    public partial class AgregarFechaFinalizacionVisita : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaFinalizacion",
                table: "VisitasTecnicas",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResultadoVisita",
                table: "VisitasTecnicas",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "VisitaCompletada",
                table: "VisitasTecnicas",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "HistorialMedidas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SolicitudCotizacionId = table.Column<int>(type: "int", nullable: false),
                    Ancho = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Alto = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Profundidad = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    UsuarioRegistro = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaRegistro = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Observaciones = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialMedidas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistorialMedidas_SolicitudesCotizacion_SolicitudCotizacionId",
                        column: x => x.SolicitudCotizacionId,
                        principalTable: "SolicitudesCotizacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialMedidas_SolicitudCotizacionId",
                table: "HistorialMedidas",
                column: "SolicitudCotizacionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistorialMedidas");

            migrationBuilder.DropColumn(
                name: "FechaFinalizacion",
                table: "VisitasTecnicas");

            migrationBuilder.DropColumn(
                name: "ResultadoVisita",
                table: "VisitasTecnicas");

            migrationBuilder.DropColumn(
                name: "VisitaCompletada",
                table: "VisitasTecnicas");
        }
    }
}

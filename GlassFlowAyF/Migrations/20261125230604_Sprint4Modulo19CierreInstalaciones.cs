using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GlassFlowAyF.Migrations
{
    /// <inheritdoc />
    public partial class Sprint4Modulo19CierreInstalaciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AprobadoPorId",
                table: "TrabajosInstalacion",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaAprobacionCierre",
                table: "TrabajosInstalacion",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaSolicitudCierre",
                table: "TrabajosInstalacion",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ObservacionCierre",
                table: "TrabajosInstalacion",
                type: "varchar(450)",
                maxLength: 450,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EvidenciasInstalacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TrabajoInstalacionId = table.Column<int>(type: "int", nullable: false),
                    NombreArchivo = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TipoContenido = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Descripcion = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UsuarioId = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaRegistro = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvidenciasInstalacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EvidenciasInstalacion_TrabajosInstalacion_TrabajoInstalacion~",
                        column: x => x.TrabajoInstalacionId,
                        principalTable: "TrabajosInstalacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "HistorialCierresInstalacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TrabajoInstalacionId = table.Column<int>(type: "int", nullable: false),
                    Accion = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Comentario = table.Column<string>(type: "varchar(450)", maxLength: 450, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UsuarioId = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Fecha = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialCierresInstalacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistorialCierresInstalacion_TrabajosInstalacion_TrabajoInsta~",
                        column: x => x.TrabajoInstalacionId,
                        principalTable: "TrabajosInstalacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_EvidenciasInstalacion_TrabajoInstalacionId",
                table: "EvidenciasInstalacion",
                column: "TrabajoInstalacionId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialCierresInstalacion_TrabajoInstalacionId_Fecha",
                table: "HistorialCierresInstalacion",
                columns: new[] { "TrabajoInstalacionId", "Fecha" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EvidenciasInstalacion");

            migrationBuilder.DropTable(
                name: "HistorialCierresInstalacion");

            migrationBuilder.DropColumn(
                name: "AprobadoPorId",
                table: "TrabajosInstalacion");

            migrationBuilder.DropColumn(
                name: "FechaAprobacionCierre",
                table: "TrabajosInstalacion");

            migrationBuilder.DropColumn(
                name: "FechaSolicitudCierre",
                table: "TrabajosInstalacion");

            migrationBuilder.DropColumn(
                name: "ObservacionCierre",
                table: "TrabajosInstalacion");
        }
    }
}

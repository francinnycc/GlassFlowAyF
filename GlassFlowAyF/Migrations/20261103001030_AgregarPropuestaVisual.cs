using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GlassFlowAyF.Migrations
{
    /// <inheritdoc />
    public partial class AgregarPropuestaVisual : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PropuestasVisuales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Codigo = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SolicitudCotizacionId = table.Column<int>(type: "int", nullable: false),
                    DisenoIAId = table.Column<int>(type: "int", nullable: true),
                    Titulo = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Descripcion = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RutaImagenPrincipal = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Estado = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    VersionMayor = table.Column<int>(type: "int", nullable: false),
                    VersionMenor = table.Column<int>(type: "int", nullable: false),
                    CreadaPor = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    RowVersion = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropuestasVisuales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropuestasVisuales_DisenosIA_DisenoIAId",
                        column: x => x.DisenoIAId,
                        principalTable: "DisenosIA",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PropuestasVisuales_SolicitudesCotizacion_SolicitudCotizacion~",
                        column: x => x.SolicitudCotizacionId,
                        principalTable: "SolicitudesCotizacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RevisionesFotografias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SolicitudFotografiaId = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Observaciones = table.Column<string>(type: "varchar(1500)", maxLength: 1500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Autor = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaRevision = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    AnalisisIA = table.Column<string>(type: "varchar(5000)", maxLength: 5000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ModeloIA = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RevisionesFotografias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RevisionesFotografias_SolicitudFotografias_SolicitudFotograf~",
                        column: x => x.SolicitudFotografiaId,
                        principalTable: "SolicitudFotografias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PropuestaVisualArchivos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PropuestaVisualId = table.Column<int>(type: "int", nullable: false),
                    RutaArchivo = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NombreOriginal = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TipoContenido = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Extension = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TamanoBytes = table.Column<long>(type: "bigint", nullable: false),
                    SubidoPor = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaCarga = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropuestaVisualArchivos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropuestaVisualArchivos_PropuestasVisuales_PropuestaVisualId",
                        column: x => x.PropuestaVisualId,
                        principalTable: "PropuestasVisuales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PropuestaVisualBitacoras",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PropuestaVisualId = table.Column<int>(type: "int", nullable: false),
                    Accion = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Detalle = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Usuario = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Fecha = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropuestaVisualBitacoras", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropuestaVisualBitacoras_PropuestasVisuales_PropuestaVisualId",
                        column: x => x.PropuestaVisualId,
                        principalTable: "PropuestasVisuales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_PropuestasVisuales_Codigo",
                table: "PropuestasVisuales",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PropuestasVisuales_DisenoIAId",
                table: "PropuestasVisuales",
                column: "DisenoIAId");

            migrationBuilder.CreateIndex(
                name: "IX_PropuestasVisuales_SolicitudCotizacionId",
                table: "PropuestasVisuales",
                column: "SolicitudCotizacionId");

            migrationBuilder.CreateIndex(
                name: "IX_PropuestaVisualArchivos_PropuestaVisualId",
                table: "PropuestaVisualArchivos",
                column: "PropuestaVisualId");

            migrationBuilder.CreateIndex(
                name: "IX_PropuestaVisualBitacoras_PropuestaVisualId",
                table: "PropuestaVisualBitacoras",
                column: "PropuestaVisualId");

            migrationBuilder.CreateIndex(
                name: "IX_RevisionesFotografias_SolicitudFotografiaId",
                table: "RevisionesFotografias",
                column: "SolicitudFotografiaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PropuestaVisualArchivos");

            migrationBuilder.DropTable(
                name: "PropuestaVisualBitacoras");

            migrationBuilder.DropTable(
                name: "RevisionesFotografias");

            migrationBuilder.DropTable(
                name: "PropuestasVisuales");
        }
    }
}

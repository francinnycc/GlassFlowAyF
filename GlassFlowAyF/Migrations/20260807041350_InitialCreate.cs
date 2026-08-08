using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GlassFlowAyF.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Productos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Categoria = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Descripcion = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PrecioBase = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productos", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SolicitudesCotizacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    NombreCliente = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Correo = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Telefono = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TipoProducto = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Descripcion = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Ancho = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    Alto = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    RequiereInstalacion = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Direccion = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Observaciones = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Estado = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaSolicitud = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    MontoEstimado = table.Column<decimal>(type: "decimal(12,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudesCotizacion", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TrabajosInstalacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Cliente = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Producto = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Direccion = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaInstalacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Instalador = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Estado = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MedidasConfirmadas = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    MaterialListo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    InstalacionRealizada = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    RevisionAcabados = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    LimpiezaFinal = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Observaciones = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrabajosInstalacion", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VisitasTecnicas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SolicitudCotizacionId = table.Column<int>(type: "int", nullable: false),
                    FechaHora = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    TecnicoAsignado = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Direccion = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Observaciones = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Estado = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisitasTecnicas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VisitasTecnicas_SolicitudesCotizacion_SolicitudCotizacionId",
                        column: x => x.SolicitudCotizacionId,
                        principalTable: "SolicitudesCotizacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Productos",
                columns: new[] { "Id", "Activo", "Categoria", "Descripcion", "Nombre", "PrecioBase" },
                values: new object[,]
                {
                    { 1, true, "Puertas", "Puerta corrediza en vidrio templado.", "Puerta de baño corrediza", 350000m },
                    { 2, true, "Ventanas", "Ventana personalizada en aluminio y vidrio.", "Ventana en vidrio", 180000m },
                    { 3, true, "Espejos", "Espejo fabricado según medidas del cliente.", "Espejo personalizado", 95000m },
                    { 4, true, "Estructuras", "Estructura personalizada para interiores.", "Estructura en vidrio", 420000m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_VisitasTecnicas_SolicitudCotizacionId",
                table: "VisitasTecnicas",
                column: "SolicitudCotizacionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Productos");

            migrationBuilder.DropTable(
                name: "TrabajosInstalacion");

            migrationBuilder.DropTable(
                name: "VisitasTecnicas");

            migrationBuilder.DropTable(
                name: "SolicitudesCotizacion");
        }
    }
}

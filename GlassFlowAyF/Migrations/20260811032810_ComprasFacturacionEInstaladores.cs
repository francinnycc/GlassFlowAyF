using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GlassFlowAyF.Migrations
{
    public partial class ComprasFacturacionEInstaladores : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Instalador",
                table: "TrabajosInstalacion");

            migrationBuilder.AlterColumn<string>(
                name: "Observaciones",
                table: "TrabajosInstalacion",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "CompraId",
                table: "TrabajosInstalacion",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaFinalizacion",
                table: "TrabajosInstalacion",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "InstalacionIniciada",
                table: "TrabajosInstalacion",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "InstaladorId",
                table: "TrabajosInstalacion",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "SolicitudCotizacionId",
                table: "TrabajosInstalacion",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Cotizaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SolicitudCotizacionId = table.Column<int>(type: "int", nullable: false),
                    CostoMateriales = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    ManoObra = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    CostoInstalacion = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    OtrosCostos = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    Subtotal = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    PorcentajeImpuesto = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Impuesto = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    Descuento = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    Estado = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DetalleTecnico = table.Column<string>(type: "varchar(1500)", maxLength: 1500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Condiciones = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ComentarioCliente = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FechaVencimiento = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FechaRespuestaCliente = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cotizaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cotizaciones_SolicitudesCotizacion_SolicitudCotizacionId",
                        column: x => x.SolicitudCotizacionId,
                        principalTable: "SolicitudesCotizacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Compras",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CotizacionId = table.Column<int>(type: "int", nullable: false),
                    ClienteId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NumeroOrden = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MetodoPago = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Estado = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Total = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    FechaCompra = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FechaPago = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Observaciones = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Compras", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Compras_AspNetUsers_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Compras_Cotizaciones_CotizacionId",
                        column: x => x.CotizacionId,
                        principalTable: "Cotizaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Facturas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CompraId = table.Column<int>(type: "int", nullable: false),
                    NumeroFactura = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaEmision = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Subtotal = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    Impuesto = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    Descuento = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    Estado = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Facturas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Facturas_Compras_CompraId",
                        column: x => x.CompraId,
                        principalTable: "Compras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Materiales",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Descripcion", "Nombre" },
                values: new object[] { null, "Vidrio templado claro 10 mm" });

            migrationBuilder.UpdateData(
                table: "Materiales",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Descripcion", "Nombre" },
                values: new object[] { null, "Vidrio templado bronce 10 mm" });

            migrationBuilder.UpdateData(
                table: "Materiales",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Descripcion", "Grosor", "Nombre" },
                values: new object[] { null, 12m, "Vidrio laminado 12 mm" });

            migrationBuilder.UpdateData(
                table: "Materiales",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Descripcion", "Nombre" },
                values: new object[] { null, "Vidrio esmerilado 8 mm" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Categoria", "Descripcion", "PrecioBase" },
                values: new object[] { "Divisiones de baño", "Sistema corredizo en vidrio templado para baño.", 185000m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Descripcion", "Nombre", "PrecioBase" },
                values: new object[] { "Ventana corrediza fabricada a medida.", "Ventana corrediza en aluminio y vidrio", 95000m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Descripcion", "PrecioBase" },
                values: new object[] { "Espejo elaborado según las medidas del espacio.", 65000m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Categoria", "Descripcion", "Nombre", "PrecioBase" },
                values: new object[] { "Barandas", "Baranda moderna en vidrio templado de seguridad.", "Baranda de vidrio templado", 190000m });

            migrationBuilder.CreateIndex(
                name: "IX_TrabajosInstalacion_CompraId",
                table: "TrabajosInstalacion",
                column: "CompraId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrabajosInstalacion_InstaladorId",
                table: "TrabajosInstalacion",
                column: "InstaladorId");

            migrationBuilder.CreateIndex(
                name: "IX_TrabajosInstalacion_SolicitudCotizacionId",
                table: "TrabajosInstalacion",
                column: "SolicitudCotizacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Compras_ClienteId",
                table: "Compras",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Compras_CotizacionId",
                table: "Compras",
                column: "CotizacionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cotizaciones_SolicitudCotizacionId",
                table: "Cotizaciones",
                column: "SolicitudCotizacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Facturas_CompraId",
                table: "Facturas",
                column: "CompraId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TrabajosInstalacion_AspNetUsers_InstaladorId",
                table: "TrabajosInstalacion",
                column: "InstaladorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_TrabajosInstalacion_Compras_CompraId",
                table: "TrabajosInstalacion",
                column: "CompraId",
                principalTable: "Compras",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_TrabajosInstalacion_SolicitudesCotizacion_SolicitudCotizacio~",
                table: "TrabajosInstalacion",
                column: "SolicitudCotizacionId",
                principalTable: "SolicitudesCotizacion",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrabajosInstalacion_AspNetUsers_InstaladorId",
                table: "TrabajosInstalacion");

            migrationBuilder.DropForeignKey(
                name: "FK_TrabajosInstalacion_Compras_CompraId",
                table: "TrabajosInstalacion");

            migrationBuilder.DropForeignKey(
                name: "FK_TrabajosInstalacion_SolicitudesCotizacion_SolicitudCotizacio~",
                table: "TrabajosInstalacion");

            migrationBuilder.DropTable(
                name: "Facturas");

            migrationBuilder.DropTable(
                name: "Compras");

            migrationBuilder.DropTable(
                name: "Cotizaciones");

            migrationBuilder.DropIndex(
                name: "IX_TrabajosInstalacion_CompraId",
                table: "TrabajosInstalacion");

            migrationBuilder.DropIndex(
                name: "IX_TrabajosInstalacion_InstaladorId",
                table: "TrabajosInstalacion");

            migrationBuilder.DropIndex(
                name: "IX_TrabajosInstalacion_SolicitudCotizacionId",
                table: "TrabajosInstalacion");

            migrationBuilder.DropColumn(
                name: "CompraId",
                table: "TrabajosInstalacion");

            migrationBuilder.DropColumn(
                name: "FechaFinalizacion",
                table: "TrabajosInstalacion");

            migrationBuilder.DropColumn(
                name: "InstalacionIniciada",
                table: "TrabajosInstalacion");

            migrationBuilder.DropColumn(
                name: "InstaladorId",
                table: "TrabajosInstalacion");

            migrationBuilder.DropColumn(
                name: "SolicitudCotizacionId",
                table: "TrabajosInstalacion");

            migrationBuilder.AlterColumn<string>(
                name: "Observaciones",
                table: "TrabajosInstalacion",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Instalador",
                table: "TrabajosInstalacion",
                type: "varchar(120)",
                maxLength: 120,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Materiales",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Descripcion", "Nombre" },
                values: new object[] { "Vidrio templado transparente de alta resistencia.", "Vidrio templado transparente" });

            migrationBuilder.UpdateData(
                table: "Materiales",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Descripcion", "Nombre" },
                values: new object[] { "Vidrio templado con tonalidad bronce.", "Vidrio templado bronce" });

            migrationBuilder.UpdateData(
                table: "Materiales",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Descripcion", "Grosor", "Nombre" },
                values: new object[] { "Vidrio laminado con mayor seguridad.", 8m, "Vidrio laminado" });

            migrationBuilder.UpdateData(
                table: "Materiales",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Descripcion", "Nombre" },
                values: new object[] { "Vidrio con acabado de privacidad.", "Vidrio esmerilado" });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Categoria", "Descripcion", "PrecioBase" },
                values: new object[] { "Puertas", "Puerta corrediza en vidrio templado.", 350000m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Descripcion", "Nombre", "PrecioBase" },
                values: new object[] { "Ventana personalizada en aluminio y vidrio.", "Ventana en vidrio", 180000m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Descripcion", "PrecioBase" },
                values: new object[] { "Espejo fabricado según las medidas del cliente.", 95000m });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Categoria", "Descripcion", "Nombre", "PrecioBase" },
                values: new object[] { "Estructuras", "Estructura personalizada para interiores.", "Estructura en vidrio", 420000m });
        }
    }
}

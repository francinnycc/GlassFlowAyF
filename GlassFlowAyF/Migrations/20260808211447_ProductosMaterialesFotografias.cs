using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GlassFlowAyF.Migrations
{
    /// <inheritdoc />
    public partial class ProductosMaterialesFotografias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Ancho",
                table: "SolicitudesCotizacion",
                type: "decimal(10,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(65,30)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Alto",
                table: "SolicitudesCotizacion",
                type: "decimal(10,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(65,30)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Cantidad",
                table: "SolicitudesCotizacion",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaterialId",
                table: "SolicitudesCotizacion",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProductoId",
                table: "SolicitudesCotizacion",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Profundidad",
                table: "SolicitudesCotizacion",
                type: "decimal(10,2)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Productos",
                type: "varchar(120)",
                maxLength: 120,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "Productos",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "Productos",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ImagenUrl",
                table: "Productos",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "PermiteInstalacion",
                table: "Productos",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Materiales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Tipo = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Color = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Perfil = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Acabado = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Grosor = table.Column<decimal>(type: "decimal(8,2)", nullable: true),
                    PrecioAdicional = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    Descripcion = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Materiales", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SolicitudFotografias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SolicitudCotizacionId = table.Column<int>(type: "int", nullable: false),
                    RutaArchivo = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NombreOriginal = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TipoContenido = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaCarga = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudFotografias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolicitudFotografias_SolicitudesCotizacion_SolicitudCotizaci~",
                        column: x => x.SolicitudCotizacionId,
                        principalTable: "SolicitudesCotizacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ProductoMateriales",
                columns: table => new
                {
                    ProductoId = table.Column<int>(type: "int", nullable: false),
                    MaterialId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductoMateriales", x => new { x.ProductoId, x.MaterialId });
                    table.ForeignKey(
                        name: "FK_ProductoMateriales_Materiales_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materiales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductoMateriales_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Materiales",
                columns: new[] { "Id", "Acabado", "Activo", "Color", "Descripcion", "Grosor", "Nombre", "Perfil", "PrecioAdicional", "Tipo" },
                values: new object[,]
                {
                    { 1, "Brillante", true, "Transparente", "Vidrio templado transparente de alta resistencia.", 10m, "Vidrio templado transparente", "Aluminio negro", 0m, "Vidrio templado" },
                    { 2, "Brillante", true, "Bronce", "Vidrio templado con tonalidad bronce.", 10m, "Vidrio templado bronce", "Aluminio negro", 25000m, "Vidrio templado" },
                    { 3, "Natural", true, "Transparente", "Vidrio laminado con mayor seguridad.", 8m, "Vidrio laminado", "Aluminio natural", 35000m, "Vidrio laminado" },
                    { 4, "Mate", true, "Esmerilado", "Vidrio con acabado de privacidad.", 8m, "Vidrio esmerilado", "Aluminio negro", 30000m, "Vidrio templado" }
                });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FechaCreacion", "ImagenUrl", "PermiteInstalacion" },
                values: new object[] { new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, true });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "FechaCreacion", "ImagenUrl", "PermiteInstalacion" },
                values: new object[] { new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, true });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Descripcion", "FechaCreacion", "ImagenUrl", "PermiteInstalacion" },
                values: new object[] { "Espejo fabricado según las medidas del cliente.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, true });

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "FechaCreacion", "ImagenUrl", "PermiteInstalacion" },
                values: new object[] { new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, true });

            migrationBuilder.InsertData(
                table: "ProductoMateriales",
                columns: new[] { "MaterialId", "ProductoId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 1 },
                    { 4, 1 },
                    { 1, 2 },
                    { 2, 2 },
                    { 3, 2 },
                    { 1, 3 },
                    { 4, 3 },
                    { 1, 4 },
                    { 3, 4 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesCotizacion_MaterialId",
                table: "SolicitudesCotizacion",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesCotizacion_ProductoId",
                table: "SolicitudesCotizacion",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductoMateriales_MaterialId",
                table: "ProductoMateriales",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudFotografias_SolicitudCotizacionId",
                table: "SolicitudFotografias",
                column: "SolicitudCotizacionId");

            migrationBuilder.AddForeignKey(
                name: "FK_SolicitudesCotizacion_Materiales_MaterialId",
                table: "SolicitudesCotizacion",
                column: "MaterialId",
                principalTable: "Materiales",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_SolicitudesCotizacion_Productos_ProductoId",
                table: "SolicitudesCotizacion",
                column: "ProductoId",
                principalTable: "Productos",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SolicitudesCotizacion_Materiales_MaterialId",
                table: "SolicitudesCotizacion");

            migrationBuilder.DropForeignKey(
                name: "FK_SolicitudesCotizacion_Productos_ProductoId",
                table: "SolicitudesCotizacion");

            migrationBuilder.DropTable(
                name: "ProductoMateriales");

            migrationBuilder.DropTable(
                name: "SolicitudFotografias");

            migrationBuilder.DropTable(
                name: "Materiales");

            migrationBuilder.DropIndex(
                name: "IX_SolicitudesCotizacion_MaterialId",
                table: "SolicitudesCotizacion");

            migrationBuilder.DropIndex(
                name: "IX_SolicitudesCotizacion_ProductoId",
                table: "SolicitudesCotizacion");

            migrationBuilder.DropColumn(
                name: "Cantidad",
                table: "SolicitudesCotizacion");

            migrationBuilder.DropColumn(
                name: "MaterialId",
                table: "SolicitudesCotizacion");

            migrationBuilder.DropColumn(
                name: "ProductoId",
                table: "SolicitudesCotizacion");

            migrationBuilder.DropColumn(
                name: "Profundidad",
                table: "SolicitudesCotizacion");

            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "ImagenUrl",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "PermiteInstalacion",
                table: "Productos");

            migrationBuilder.AlterColumn<decimal>(
                name: "Ancho",
                table: "SolicitudesCotizacion",
                type: "decimal(65,30)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Alto",
                table: "SolicitudesCotizacion",
                type: "decimal(65,30)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Productos",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(120)",
                oldMaxLength: 120)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "Productos",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 3,
                column: "Descripcion",
                value: "Espejo fabricado según medidas del cliente.");
        }
    }
}

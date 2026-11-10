
using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace GlassFlowAyF.Migrations
{
    public partial class Modulo17Produccion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // La tabla OrdenesProduccion ya existe porque MySQL
            // conservó su creación durante la migración fallida.
            //
            // RevisionesFotografias también existe y pertenece
            // a una funcionalidad anterior.
            //
            // No se recrea ni se modifica ninguna de esas tablas.

            // Crear la tabla pendiente de materiales de producción.
            migrationBuilder.CreateTable(
                name: "OrdenesProduccionMateriales",
                columns: table => new
                {
                    Id = table.Column<int>(
                        type: "int",
                        nullable: false)
                        .Annotation(
                            "MySql:ValueGenerationStrategy",
                            MySqlValueGenerationStrategy.IdentityColumn),

                    OrdenProduccionId = table.Column<int>(
                        type: "int",
                        nullable: false),

                    MaterialId = table.Column<int>(
                        type: "int",
                        nullable: false),

                    Cantidad = table.Column<decimal>(
                        type: "decimal(12,2)",
                        nullable: false),

                    Unidad = table.Column<string>(
                        type: "varchar(30)",
                        maxLength: 30,
                        nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),

                    ConfirmadoDisponible = table.Column<bool>(
                        type: "tinyint(1)",
                        nullable: false),

                    Observaciones = table.Column<string>(
                        type: "varchar(500)",
                        maxLength: 500,
                        nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_OrdenesProduccionMateriales",
                        x => x.Id);

                    table.ForeignKey(
                        name: "FK_OrdenesProduccionMateriales_Materiales_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materiales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);

                    table.ForeignKey(
                        name: "FK_OrdenesProduccionMateriales_OrdenesProduccion_OrdenProduccio~",
                        column: x => x.OrdenProduccionId,
                        principalTable: "OrdenesProduccion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            // Índices de la tabla de órdenes ya existente.
            migrationBuilder.CreateIndex(
                name: "IX_OrdenesProduccion_CompraId",
                table: "OrdenesProduccion",
                column: "CompraId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesProduccion_NumeroOrden",
                table: "OrdenesProduccion",
                column: "NumeroOrden",
                unique: true);

            // Índices de los materiales de producción.
            migrationBuilder.CreateIndex(
                name: "IX_OrdenesProduccionMateriales_MaterialId",
                table: "OrdenesProduccionMateriales",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesProduccionMateriales_OrdenProduccionId_MaterialId",
                table: "OrdenesProduccionMateriales",
                columns: new[]
                {
                    "OrdenProduccionId",
                    "MaterialId"
                },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revertir únicamente los cambios aplicados
            // por esta versión corregida de la migración.
            migrationBuilder.DropTable(
                name: "OrdenesProduccionMateriales");

            migrationBuilder.DropIndex(
                name: "IX_OrdenesProduccion_CompraId",
                table: "OrdenesProduccion");

            migrationBuilder.DropIndex(
                name: "IX_OrdenesProduccion_NumeroOrden",
                table: "OrdenesProduccion");

            // No eliminar OrdenesProduccion porque ya existía
            // antes de aplicar esta versión corregida.
            //
            // No eliminar RevisionesFotografias porque pertenece
            // a un módulo anterior.
        }
    }
}

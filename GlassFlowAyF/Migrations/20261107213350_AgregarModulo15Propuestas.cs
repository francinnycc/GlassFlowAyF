using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GlassFlowAyF.Migrations
{
    /// <inheritdoc />
    public partial class AgregarModulo15Propuestas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FichaTecnicaUrl",
                table: "Materiales",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Garantia",
                table: "Materiales",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Herrajes",
                table: "Materiales",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "Congelada",
                table: "Cotizaciones",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CongeladaPorId",
                table: "Cotizaciones",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCongelacion",
                table: "Cotizaciones",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IpCongelacion",
                table: "Cotizaciones",
                type: "varchar(64)",
                maxLength: 64,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "PlanosUrl",
                table: "Cotizaciones",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DecisionesPropuesta",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SolicitudCotizacionId = table.Column<int>(type: "int", nullable: false),
                    CotizacionId = table.Column<int>(type: "int", nullable: false),
                    DisenoIAId = table.Column<int>(type: "int", nullable: true),
                    ClienteId = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Decision = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Comentario = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AceptaTerminos = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    FechaHora = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DireccionIP = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    VersionCotizacion = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    EstadoAnterior = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EstadoNuevo = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DecisionesPropuesta", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DecisionesPropuesta_AspNetUsers_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DecisionesPropuesta_Cotizaciones_CotizacionId",
                        column: x => x.CotizacionId,
                        principalTable: "Cotizaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DecisionesPropuesta_DisenosIA_DisenoIAId",
                        column: x => x.DisenoIAId,
                        principalTable: "DisenosIA",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_DecisionesPropuesta_SolicitudesCotizacion_SolicitudCotizacio~",
                        column: x => x.SolicitudCotizacionId,
                        principalTable: "SolicitudesCotizacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Notificaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UsuarioId = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RolDestino = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Titulo = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Mensaje = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Tipo = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Referencia = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Leida = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notificaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notificaciones_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Materiales",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FichaTecnicaUrl", "Garantia", "Herrajes" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Materiales",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "FichaTecnicaUrl", "Garantia", "Herrajes" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Materiales",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "FichaTecnicaUrl", "Garantia", "Herrajes" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Materiales",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "FichaTecnicaUrl", "Garantia", "Herrajes" },
                values: new object[] { null, null, null });

            migrationBuilder.CreateIndex(
                name: "IX_Cotizaciones_CongeladaPorId",
                table: "Cotizaciones",
                column: "CongeladaPorId");

            migrationBuilder.CreateIndex(
                name: "IX_DecisionesPropuesta_ClienteId",
                table: "DecisionesPropuesta",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_DecisionesPropuesta_CotizacionId",
                table: "DecisionesPropuesta",
                column: "CotizacionId");

            migrationBuilder.CreateIndex(
                name: "IX_DecisionesPropuesta_DisenoIAId",
                table: "DecisionesPropuesta",
                column: "DisenoIAId");

            migrationBuilder.CreateIndex(
                name: "IX_DecisionesPropuesta_SolicitudCotizacionId_FechaHora",
                table: "DecisionesPropuesta",
                columns: new[] { "SolicitudCotizacionId", "FechaHora" });

            migrationBuilder.CreateIndex(
                name: "IX_Notificaciones_Leida_FechaCreacion",
                table: "Notificaciones",
                columns: new[] { "Leida", "FechaCreacion" });

            migrationBuilder.CreateIndex(
                name: "IX_Notificaciones_UsuarioId",
                table: "Notificaciones",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cotizaciones_AspNetUsers_CongeladaPorId",
                table: "Cotizaciones",
                column: "CongeladaPorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cotizaciones_AspNetUsers_CongeladaPorId",
                table: "Cotizaciones");

            migrationBuilder.DropTable(
                name: "DecisionesPropuesta");

            migrationBuilder.DropTable(
                name: "Notificaciones");

            migrationBuilder.DropIndex(
                name: "IX_Cotizaciones_CongeladaPorId",
                table: "Cotizaciones");

            migrationBuilder.DropColumn(
                name: "FichaTecnicaUrl",
                table: "Materiales");

            migrationBuilder.DropColumn(
                name: "Garantia",
                table: "Materiales");

            migrationBuilder.DropColumn(
                name: "Herrajes",
                table: "Materiales");

            migrationBuilder.DropColumn(
                name: "Congelada",
                table: "Cotizaciones");

            migrationBuilder.DropColumn(
                name: "CongeladaPorId",
                table: "Cotizaciones");

            migrationBuilder.DropColumn(
                name: "FechaCongelacion",
                table: "Cotizaciones");

            migrationBuilder.DropColumn(
                name: "IpCongelacion",
                table: "Cotizaciones");

            migrationBuilder.DropColumn(
                name: "PlanosUrl",
                table: "Cotizaciones");
        }
    }
}

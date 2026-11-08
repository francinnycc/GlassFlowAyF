using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GlassFlowAyF.Migrations
{
    /// <inheritdoc />
    public partial class Modulo15RevisionCliente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Activa",
                table: "Cotizaciones",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "Congelada",
                table: "Cotizaciones",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "DisenoIAId",
                table: "Cotizaciones",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCongelacion",
                table: "Cotizaciones",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SnapshotHash",
                table: "Cotizaciones",
                type: "varchar(64)",
                maxLength: 64,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Notificaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DestinatarioRol = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Titulo = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Mensaje = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Url = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Tipo = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SolicitudCotizacionId = table.Column<int>(type: "int", nullable: true),
                    Leida = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notificaciones", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "OrdenesPreliminares",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CotizacionId = table.Column<int>(type: "int", nullable: false),
                    SolicitudCotizacionId = table.Column<int>(type: "int", nullable: false),
                    Numero = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Estado = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenesPreliminares", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrdenesPreliminares_Cotizaciones_CotizacionId",
                        column: x => x.CotizacionId,
                        principalTable: "Cotizaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesPreliminares_SolicitudesCotizacion_SolicitudCotizacio~",
                        column: x => x.SolicitudCotizacionId,
                        principalTable: "SolicitudesCotizacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RevisionesCliente",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CotizacionId = table.Column<int>(type: "int", nullable: false),
                    SolicitudCotizacionId = table.Column<int>(type: "int", nullable: false),
                    DisenoIAId = table.Column<int>(type: "int", nullable: true),
                    UsuarioId = table.Column<string>(type: "varchar(450)", maxLength: 450, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Decision = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Comentario = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EstadoSolicitudAnterior = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EstadoSolicitudNuevo = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EstadoCotizacionAnterior = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EstadoCotizacionNuevo = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    VersionCotizacion = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    AceptoTerminos = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DireccionIp = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserAgent = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    SnapshotJson = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RevisionesCliente", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RevisionesCliente_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RevisionesCliente_Cotizaciones_CotizacionId",
                        column: x => x.CotizacionId,
                        principalTable: "Cotizaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RevisionesCliente_DisenosIA_DisenoIAId",
                        column: x => x.DisenoIAId,
                        principalTable: "DisenosIA",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_RevisionesCliente_SolicitudesCotizacion_SolicitudCotizacionId",
                        column: x => x.SolicitudCotizacionId,
                        principalTable: "SolicitudesCotizacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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

            migrationBuilder.CreateIndex(
                name: "IX_Cotizaciones_DisenoIAId",
                table: "Cotizaciones",
                column: "DisenoIAId");

            migrationBuilder.CreateIndex(
                name: "IX_Notificaciones_DestinatarioRol_Leida",
                table: "Notificaciones",
                columns: new[] { "DestinatarioRol", "Leida" });

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesPreliminares_CotizacionId",
                table: "OrdenesPreliminares",
                column: "CotizacionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesPreliminares_Numero",
                table: "OrdenesPreliminares",
                column: "Numero",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesPreliminares_SolicitudCotizacionId",
                table: "OrdenesPreliminares",
                column: "SolicitudCotizacionId");

            migrationBuilder.CreateIndex(
                name: "IX_RevisionesCliente_CotizacionId_VersionCotizacion",
                table: "RevisionesCliente",
                columns: new[] { "CotizacionId", "VersionCotizacion" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RevisionesCliente_DisenoIAId",
                table: "RevisionesCliente",
                column: "DisenoIAId");

            migrationBuilder.CreateIndex(
                name: "IX_RevisionesCliente_FechaUtc",
                table: "RevisionesCliente",
                column: "FechaUtc");

            migrationBuilder.CreateIndex(
                name: "IX_RevisionesCliente_SolicitudCotizacionId",
                table: "RevisionesCliente",
                column: "SolicitudCotizacionId");

            migrationBuilder.CreateIndex(
                name: "IX_RevisionesCliente_UsuarioId",
                table: "RevisionesCliente",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_RevisionesFotografias_SolicitudFotografiaId",
                table: "RevisionesFotografias",
                column: "SolicitudFotografiaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cotizaciones_DisenosIA_DisenoIAId",
                table: "Cotizaciones",
                column: "DisenoIAId",
                principalTable: "DisenosIA",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            // ---- Datos existentes (Módulo 15) ----
            // Las cotizaciones rechazadas quedan inactivas y las aprobadas, congeladas.
            migrationBuilder.Sql("UPDATE `Cotizaciones` SET `Activa` = 0 WHERE `Estado` = 'Rechazada';");
            migrationBuilder.Sql("UPDATE `Cotizaciones` SET `Congelada` = 1, `FechaCongelacion` = COALESCE(`FechaRespuestaCliente`, NOW()) WHERE `Estado` = 'Aprobada';");
            // Nuevo nombre del estado de proyecto aprobado.
            migrationBuilder.Sql("UPDATE `SolicitudesCotizacion` SET `Estado` = 'Aprobado por el Cliente' WHERE `Estado` = 'Aprobado';");
            // Las aprobaciones previas obtienen su orden preliminar para poder avanzar a producción.
            migrationBuilder.Sql(@"INSERT INTO `OrdenesPreliminares` (`CotizacionId`, `SolicitudCotizacionId`, `Numero`, `Estado`, `FechaCreacion`)
SELECT c.`Id`, c.`SolicitudCotizacionId`, CONCAT('GF-OP-', YEAR(NOW()), '-M', LPAD(c.`Id`, 5, '0')), 'Preliminar', NOW()
FROM `Cotizaciones` c
WHERE c.`Estado` = 'Aprobada'
  AND NOT EXISTS (SELECT 1 FROM `OrdenesPreliminares` o WHERE o.`CotizacionId` = c.`Id`);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE `SolicitudesCotizacion` SET `Estado` = 'Aprobado' WHERE `Estado` = 'Aprobado por el Cliente';");

            migrationBuilder.DropForeignKey(
                name: "FK_Cotizaciones_DisenosIA_DisenoIAId",
                table: "Cotizaciones");

            migrationBuilder.DropTable(
                name: "Notificaciones");

            migrationBuilder.DropTable(
                name: "OrdenesPreliminares");

            migrationBuilder.DropTable(
                name: "RevisionesCliente");

            migrationBuilder.DropTable(
                name: "RevisionesFotografias");

            migrationBuilder.DropIndex(
                name: "IX_Cotizaciones_DisenoIAId",
                table: "Cotizaciones");

            migrationBuilder.DropColumn(
                name: "Activa",
                table: "Cotizaciones");

            migrationBuilder.DropColumn(
                name: "Congelada",
                table: "Cotizaciones");

            migrationBuilder.DropColumn(
                name: "DisenoIAId",
                table: "Cotizaciones");

            migrationBuilder.DropColumn(
                name: "FechaCongelacion",
                table: "Cotizaciones");

            migrationBuilder.DropColumn(
                name: "SnapshotHash",
                table: "Cotizaciones");
        }
    }
}

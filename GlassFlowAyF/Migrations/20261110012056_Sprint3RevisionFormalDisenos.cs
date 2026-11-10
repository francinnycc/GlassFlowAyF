
using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GlassFlowAyF.Migrations
{
    /// <inheritdoc />
    public partial class Sprint3RevisionFormalDisenos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ComentarioRevision",
                table: "DisenosIA",
                type: "varchar(2000)",
                maxLength: 2000,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "EstadoRevision",
                table: "DisenosIA",
                type: "varchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "Pendiente")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaUltimaRevision",
                table: "DisenosIA",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioUltimaRevisionId",
                table: "DisenosIA",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DisenosIARevisiones",
                columns: table => new
                {
                    Id = table.Column<int>(
                        type: "int",
                        nullable: false)
                        .Annotation(
                            "MySql:ValueGenerationStrategy",
                            MySqlValueGenerationStrategy.IdentityColumn),

                    DisenoIAId = table.Column<int>(
                        type: "int",
                        nullable: false),

                    Estado = table.Column<string>(
                        type: "varchar(30)",
                        maxLength: 30,
                        nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),

                    Comentario = table.Column<string>(
                        type: "varchar(2000)",
                        maxLength: 2000,
                        nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),

                    UsuarioId = table.Column<string>(
                        type: "varchar(450)",
                        maxLength: 450,
                        nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),

                    Fecha = table.Column<DateTime>(
                        type: "datetime(6)",
                        nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_DisenosIARevisiones",
                        x => x.Id);

                    table.ForeignKey(
                        name: "FK_DisenosIARevisiones_DisenosIA_DisenoIAId",
                        column: x => x.DisenoIAId,
                        principalTable: "DisenosIA",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_DisenosIARevisiones_DisenoIAId",
                table: "DisenosIARevisiones",
                column: "DisenoIAId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DisenosIARevisiones");

            migrationBuilder.DropColumn(
                name: "ComentarioRevision",
                table: "DisenosIA");

            migrationBuilder.DropColumn(
                name: "EstadoRevision",
                table: "DisenosIA");

            migrationBuilder.DropColumn(
                name: "FechaUltimaRevision",
                table: "DisenosIA");

            migrationBuilder.DropColumn(
                name: "UsuarioUltimaRevisionId",
                table: "DisenosIA");
        }
    }
}

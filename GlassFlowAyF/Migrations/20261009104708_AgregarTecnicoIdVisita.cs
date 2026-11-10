using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GlassFlowAyF.Migrations
{
    /// <inheritdoc />
    public partial class AgregarTecnicoIdVisita : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TecnicoId",
                table: "VisitasTecnicas",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_VisitasTecnicas_TecnicoId",
                table: "VisitasTecnicas",
                column: "TecnicoId");

            migrationBuilder.AddForeignKey(
                name: "FK_VisitasTecnicas_AspNetUsers_TecnicoId",
                table: "VisitasTecnicas",
                column: "TecnicoId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VisitasTecnicas_AspNetUsers_TecnicoId",
                table: "VisitasTecnicas");

            migrationBuilder.DropIndex(
                name: "IX_VisitasTecnicas_TecnicoId",
                table: "VisitasTecnicas");

            migrationBuilder.DropColumn(
                name: "TecnicoId",
                table: "VisitasTecnicas");
        }
    }
}

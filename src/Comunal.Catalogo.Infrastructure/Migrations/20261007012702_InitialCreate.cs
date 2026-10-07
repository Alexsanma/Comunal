using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Comunal.Catalogo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Objetos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CategoriaId = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FechaPublicacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PropietarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Objetos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Objetos_Categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Categorias",
                columns: new[] { "Id", "Nombre" },
                values: new object[,]
                {
                    { 1, "Herramientas" },
                    { 2, "Hogar" },
                    { 3, "Electrónica" },
                    { 4, "Libros" },
                    { 5, "Deportes" }
                });

            migrationBuilder.InsertData(
                table: "Objetos",
                columns: new[] { "Id", "CategoriaId", "Descripcion", "Estado", "FechaPublicacion", "Nombre", "PropietarioId" },
                values: new object[,]
                {
                    { 1, 1, "Taladro con maletín y set de brocas.", "Disponible", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Taladro percutor Bosch", new Guid("11111111-1111-1111-1111-111111111111") },
                    { 2, 5, "Carpa impermeable, ideal para camping.", "Disponible", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Carpa para 4 personas", new Guid("11111111-1111-1111-1111-111111111111") },
                    { 3, 2, "Greca de aluminio para 6 tazas.", "Disponible", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Cafetera italiana", new Guid("11111111-1111-1111-1111-111111111111") },
                    { 4, 3, "Monitor Full HD con cable HDMI.", "Prestado", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Monitor 24 pulgadas", new Guid("11111111-1111-1111-1111-111111111111") },
                    { 5, 4, "Novela de J. R. R. Tolkien, edición de bolsillo.", "Disponible", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "El Hobbit", new Guid("11111111-1111-1111-1111-111111111111") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_Nombre",
                table: "Categorias",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Objetos_CategoriaId",
                table: "Objetos",
                column: "CategoriaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Objetos");

            migrationBuilder.DropTable(
                name: "Categorias");
        }
    }
}

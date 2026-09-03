using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiarioDoCoelho.Migrations
{
    /// <inheritdoc />
    public partial class AddBauDoCoelhoCategoria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categorias",
                columns: new[] { "Id", "Nome" },
                values: new object[] { 5, "Baú do Coelho" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categorias",
                keyColumn: "Id",
                keyValue: 5);
        }
    }
}

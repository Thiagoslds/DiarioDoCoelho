using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiarioDoCoelho.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoriaELinkTabelaToJogo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Categoria",
                table: "Jogos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "LinkTabela",
                table: "Jogos",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Categoria",
                table: "Jogos");

            migrationBuilder.DropColumn(
                name: "LinkTabela",
                table: "Jogos");
        }
    }
}

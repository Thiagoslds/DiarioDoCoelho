using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiarioDoCoelho.Migrations
{
    /// <inheritdoc />
    public partial class AddTipoPostToPost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TipoPost",
                table: "Posts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TipoPost",
                table: "Posts");
        }
    }
}

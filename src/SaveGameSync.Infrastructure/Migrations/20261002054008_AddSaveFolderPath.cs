using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SaveGameSync.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSaveFolderPath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SaveFolderPath",
                table: "Games",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SaveFolderPath",
                table: "Games");
        }
    }
}

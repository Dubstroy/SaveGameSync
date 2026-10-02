using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SaveGameSync.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTestMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TestMigration",
                table: "Games",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TestMigration",
                table: "Games");
        }
    }
}

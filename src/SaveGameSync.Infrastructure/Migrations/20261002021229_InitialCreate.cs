using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SaveGameSync.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<Guid>(
                        type: "TEXT",
                        nullable: false),

                    Name = table.Column<string>(
                        type: "TEXT",
                        nullable: false),

                    Origin = table.Column<string>(
                        type: "TEXT",
                        nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SaveVersions",
                columns: table => new
                {
                    Id = table.Column<Guid>(
                        type: "TEXT",
                        nullable: false),

                    BackupPath = table.Column<string>(
                        type: "TEXT",
                        nullable: false),

                    CreatedAt = table.Column<DateTimeOffset>(
                        type: "TEXT",
                        nullable: false),

                    FileHash = table.Column<string>(
                        type: "TEXT",
                        nullable: false),

                    TrackedSaveId = table.Column<Guid>(
                        type: "TEXT",
                        nullable: false),

                    VersionNumber = table.Column<int>(
                        type: "INTEGER",
                        nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaveVersions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TrackedSaves",
                columns: table => new
                {
                    Id = table.Column<Guid>(
                        type: "TEXT",
                        nullable: false),

                    CurrentHash = table.Column<string>(
                        type: "TEXT",
                        nullable: true),

                    Exists = table.Column<bool>(
                        type: "INTEGER",
                        nullable: false),

                    FilePath = table.Column<string>(
                        type: "TEXT",
                        nullable: false),

                    GameId = table.Column<Guid>(
                        type: "TEXT",
                        nullable: false),

                    LastDetectedAt = table.Column<DateTimeOffset?>(
                        type: "TEXT",
                        nullable: true),

                    Name = table.Column<string>(
                        type: "TEXT",
                        nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackedSaves", x => x.Id);

                    table.ForeignKey(
                        name: "FK_TrackedSaves_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SaveVersions_TrackedSaveId_VersionNumber",
                table: "SaveVersions",
                columns: new[] { "TrackedSaveId", "VersionNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrackedSaves_GameId",
                table: "TrackedSaves",
                column: "GameId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SaveVersions");

            migrationBuilder.DropTable(
                name: "TrackedSaves");

            migrationBuilder.DropTable(
                name: "Games");
        }
    }
}
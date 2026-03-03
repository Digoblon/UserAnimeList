using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserAnimeList.Infrastructure.Data.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddingAnimeListTableanditsrelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserAnimeLists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AnimeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: true),
                    Progress = table.Column<int>(type: "int", nullable: true),
                    DateStarted = table.Column<DateOnly>(type: "date", nullable: true),
                    DateFinished = table.Column<DateOnly>(type: "date", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAnimeLists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAnimeLists_Animes_AnimeId",
                        column: x => x.AnimeId,
                        principalTable: "Animes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserAnimeLists_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserAnimeLists_AnimeId",
                table: "UserAnimeLists",
                column: "AnimeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAnimeLists_Status",
                table: "UserAnimeLists",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_UserAnimeLists_UserId",
                table: "UserAnimeLists",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAnimeLists_UserId_AnimeId",
                table: "UserAnimeLists",
                columns: new[] { "UserId", "AnimeId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserAnimeLists");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserAnimeList.Infrastructure.Data.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddingNameNormalizedtoStudiotable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Studios_Name",
                table: "Studios");

            migrationBuilder.AddColumn<string>(
                name: "NameNormalized",
                table: "Studios",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Studios_NameNormalized",
                table: "Studios",
                column: "NameNormalized",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Studios_NameNormalized",
                table: "Studios");

            migrationBuilder.DropColumn(
                name: "NameNormalized",
                table: "Studios");

            migrationBuilder.CreateIndex(
                name: "IX_Studios_Name",
                table: "Studios",
                column: "Name",
                unique: true);
        }
    }
}

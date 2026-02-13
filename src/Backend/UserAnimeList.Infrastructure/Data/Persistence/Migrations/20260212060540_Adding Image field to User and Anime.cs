using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserAnimeList.Infrastructure.Data.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddingImagefieldtoUserandAnime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Animes",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Animes");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmreCakmakoglu.Migrations
{
    /// <inheritdoc />
    public partial class AddMusicPlatformLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AppleMusicUrl",
                table: "Musics",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "YoutubeUrl",
                table: "Musics",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AppleMusicUrl",
                table: "Musics");

            migrationBuilder.DropColumn(
                name: "YoutubeUrl",
                table: "Musics");
        }
    }
}

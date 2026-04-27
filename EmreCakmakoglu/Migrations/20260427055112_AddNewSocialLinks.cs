using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmreCakmakoglu.Migrations
{
    /// <inheritdoc />
    public partial class AddNewSocialLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BlueSkyUrl",
                table: "SiteSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FacebookUrl",
                table: "SiteSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "YouTubeMusicUrl",
                table: "SiteSettings",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlueSkyUrl",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "FacebookUrl",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "YouTubeMusicUrl",
                table: "SiteSettings");
        }
    }
}

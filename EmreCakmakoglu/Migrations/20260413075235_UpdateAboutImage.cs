using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmreCakmakoglu.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAboutImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AboutImageUrl",
                table: "SiteSettings",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AboutImageUrl",
                table: "SiteSettings");
        }
    }
}

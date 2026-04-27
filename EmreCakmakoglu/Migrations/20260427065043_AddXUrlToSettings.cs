using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmreCakmakoglu.Migrations
{
    /// <inheritdoc />
    public partial class AddXUrlToSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "XUrl",
                table: "SiteSettings",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "XUrl",
                table: "SiteSettings");
        }
    }
}

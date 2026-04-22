using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmreCakmakoglu.Migrations
{
    /// <inheritdoc />
    public partial class AddMonthToMusic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Month",
                table: "Musics",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Month",
                table: "Musics");
        }
    }
}

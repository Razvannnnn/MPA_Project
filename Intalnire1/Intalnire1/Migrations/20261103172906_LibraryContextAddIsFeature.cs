using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Intalnire1.Migrations
{
    /// <inheritdoc />
    public partial class LibraryContextAddIsFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFeatured",
                table: "Book",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFeatured",
                table: "Book");
        }
    }
}

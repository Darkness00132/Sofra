using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sofra.Infrastructure.Persistent.Migrations
{
    /// <inheritdoc />
    public partial class AddMenuItemImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePublicId",
                table: "FoodItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "FoodItems",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePublicId",
                table: "FoodItems");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "FoodItems");
        }
    }
}

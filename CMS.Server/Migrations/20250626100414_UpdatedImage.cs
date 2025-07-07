using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMS.Server.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_images_colors_colorid",
                table: "images");

            migrationBuilder.DropIndex(
                name: "IX_images_colorid",
                table: "images");

            migrationBuilder.DropColumn(
                name: "availiablestock",
                table: "colors");

            migrationBuilder.AddColumn<int>(
                name: "clothid",
                table: "images",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "availablestock",
                table: "clothcolors",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_images_clothid_colorid",
                table: "images",
                columns: new[] { "clothid", "colorid" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_images_clothcolors_clothid_colorid",
                table: "images",
                columns: new[] { "clothid", "colorid" },
                principalTable: "clothcolors",
                principalColumns: new[] { "clothid", "colorid" },
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_images_clothcolors_clothid_colorid",
                table: "images");

            migrationBuilder.DropIndex(
                name: "IX_images_clothid_colorid",
                table: "images");

            migrationBuilder.DropColumn(
                name: "clothid",
                table: "images");

            migrationBuilder.DropColumn(
                name: "availablestock",
                table: "clothcolors");

            migrationBuilder.AddColumn<int>(
                name: "availiablestock",
                table: "colors",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_images_colorid",
                table: "images",
                column: "colorid",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_images_colors_colorid",
                table: "images",
                column: "colorid",
                principalTable: "colors",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

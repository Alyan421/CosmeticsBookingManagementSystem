using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMS.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddClothColorManyToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_colors_cloths_clothid",
                table: "colors");

            migrationBuilder.DropIndex(
                name: "IX_colors_clothid",
                table: "colors");

            migrationBuilder.DropColumn(
                name: "clothid",
                table: "colors");

            migrationBuilder.CreateTable(
                name: "clothcolors",
                columns: table => new
                {
                    clothid = table.Column<int>(type: "integer", nullable: false),
                    colorid = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clothcolors", x => new { x.clothid, x.colorid });
                    table.ForeignKey(
                        name: "FK_clothcolors_cloths_clothid",
                        column: x => x.clothid,
                        principalTable: "cloths",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_clothcolors_colors_colorid",
                        column: x => x.colorid,
                        principalTable: "colors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_clothcolors_colorid",
                table: "clothcolors",
                column: "colorid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "clothcolors");

            migrationBuilder.AddColumn<int>(
                name: "clothid",
                table: "colors",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_colors_clothid",
                table: "colors",
                column: "clothid");

            migrationBuilder.AddForeignKey(
                name: "FK_colors_cloths_clothid",
                table: "colors",
                column: "clothid",
                principalTable: "cloths",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

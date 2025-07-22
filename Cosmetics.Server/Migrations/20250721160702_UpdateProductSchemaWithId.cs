using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Cosmetics.Server.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProductSchemaWithId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // First, drop the foreign key constraint from images table
            migrationBuilder.DropForeignKey(
                name: "FK_images_products_brandid_categoryid",
                table: "images");

            // Drop the unique index on images table
            migrationBuilder.DropIndex(
                name: "IX_images_brandid_categoryid",
                table: "images");

            // Drop the current primary key on products table
            migrationBuilder.DropPrimaryKey(
                name: "PK_products",
                table: "products");

            // Remove brandid and categoryid columns from images table
            migrationBuilder.DropColumn(
                name: "brandid",
                table: "images");

            migrationBuilder.DropColumn(
                name: "categoryid",
                table: "images");

            // Add the new id column to products table as primary key
            migrationBuilder.AddColumn<int>(
                name: "id",
                table: "products",
                type: "integer",
                nullable: false)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            // Add productname column (required)
            migrationBuilder.AddColumn<string>(
                name: "productname",
                table: "products",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "Default Product"); // Provide a default value for existing records

            // Add description column (optional)
            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "products",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            // Add productid column to images table
            migrationBuilder.AddColumn<int>(
                name: "productid",
                table: "images",
                type: "integer",
                nullable: false,
                defaultValue: 1); // Temporary default value

            // Set the new primary key on products table
            migrationBuilder.AddPrimaryKey(
                name: "PK_products",
                table: "products",
                column: "id");

            // Create index for better query performance on products table
            migrationBuilder.CreateIndex(
                name: "IX_products_brandid_categoryid",
                table: "products",
                columns: new[] { "brandid", "categoryid" });

            // Create unique index on images.productid
            migrationBuilder.CreateIndex(
                name: "IX_images_productid",
                table: "images",
                column: "productid",
                unique: true);

            // Add the new foreign key constraint
            migrationBuilder.AddForeignKey(
                name: "FK_images_products_productid",
                table: "images",
                column: "productid",
                principalTable: "products",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            // Update existing data: Set productid in images table to match the first product with same brand/category
            // Note: This is a simplified approach - you may need to customize based on your data
            migrationBuilder.Sql(@"
                UPDATE images 
                SET productid = (
                    SELECT MIN(p.id) 
                    FROM products p 
                    WHERE p.brandid = (
                        SELECT brandid FROM products 
                        WHERE brandid IS NOT NULL 
                        LIMIT 1
                    ) 
                    AND p.categoryid = (
                        SELECT categoryid FROM products 
                        WHERE categoryid IS NOT NULL 
                        LIMIT 1
                    )
                    LIMIT 1
                )
                WHERE productid = 1;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop the new foreign key
            migrationBuilder.DropForeignKey(
                name: "FK_images_products_productid",
                table: "images");

            // Drop the new primary key
            migrationBuilder.DropPrimaryKey(
                name: "PK_products",
                table: "products");

            // Drop indexes
            migrationBuilder.DropIndex(
                name: "IX_products_brandid_categoryid",
                table: "products");

            migrationBuilder.DropIndex(
                name: "IX_images_productid",
                table: "images");

            // Remove new columns from products table
            migrationBuilder.DropColumn(
                name: "id",
                table: "products");

            migrationBuilder.DropColumn(
                name: "description",
                table: "products");

            migrationBuilder.DropColumn(
                name: "productname",
                table: "products");

            // Remove productid column from images table
            migrationBuilder.DropColumn(
                name: "productid",
                table: "images");

            // Add back brandid and categoryid columns to images table
            migrationBuilder.AddColumn<int>(
                name: "brandid",
                table: "images",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "categoryid",
                table: "images",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            // Restore the composite primary key on products table
            migrationBuilder.AddPrimaryKey(
                name: "PK_products",
                table: "products",
                columns: new[] { "brandid", "categoryid" });

            // Restore the unique index on images table
            migrationBuilder.CreateIndex(
                name: "IX_images_brandid_categoryid",
                table: "images",
                columns: new[] { "brandid", "categoryid" },
                unique: true);

            // Restore the original foreign key
            migrationBuilder.AddForeignKey(
                name: "FK_images_products_brandid_categoryid",
                table: "images",
                columns: new[] { "brandid", "categoryid" },
                principalTable: "products",
                principalColumns: new[] { "brandid", "categoryid" },
                onDelete: ReferentialAction.Cascade);
        }
    }
}
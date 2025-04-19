using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddedAdminApprovalByAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_product_image_entity_products_product_id",
                table: "product_image_entity");

            migrationBuilder.DropPrimaryKey(
                name: "pk_product_image_entity",
                table: "product_image_entity");

            migrationBuilder.RenameTable(
                name: "product_image_entity",
                newName: "product_images");

            migrationBuilder.RenameIndex(
                name: "ix_product_image_entity_product_id",
                table: "product_images",
                newName: "ix_product_images_product_id");

            migrationBuilder.AddColumn<bool>(
                name: "is_approved_by_admin",
                table: "users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "pk_product_images",
                table: "product_images",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_product_images_products_product_id",
                table: "product_images",
                column: "product_id",
                principalTable: "products",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_product_images_products_product_id",
                table: "product_images");

            migrationBuilder.DropPrimaryKey(
                name: "pk_product_images",
                table: "product_images");

            migrationBuilder.DropColumn(
                name: "is_approved_by_admin",
                table: "users");

            migrationBuilder.RenameTable(
                name: "product_images",
                newName: "product_image_entity");

            migrationBuilder.RenameIndex(
                name: "ix_product_images_product_id",
                table: "product_image_entity",
                newName: "ix_product_image_entity_product_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_product_image_entity",
                table: "product_image_entity",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_product_image_entity_products_product_id",
                table: "product_image_entity",
                column: "product_id",
                principalTable: "products",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

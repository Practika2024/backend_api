using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class Added_is_viewed_to_reminder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "status",
                table: "reminders");

            migrationBuilder.AddColumn<bool>(
                name: "is_viewed",
                table: "reminders",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_viewed",
                table: "reminders");

            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "reminders",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}

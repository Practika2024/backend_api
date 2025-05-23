using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddedToReminderStatusAndTypeAsEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "type",
                table: "reminders",
                newName: "type_id");

            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "reminders",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "reminder_types",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_by = table.Column<Guid>(type: "uuid", nullable: true),
                    modified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_reminder_types", x => x.id);
                    table.ForeignKey(
                        name: "fk_reminder_types_users_created_by",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_reminder_types_users_modified_by",
                        column: x => x.modified_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_reminders_type_id",
                table: "reminders",
                column: "type_id");

            migrationBuilder.CreateIndex(
                name: "ix_reminder_types_created_by",
                table: "reminder_types",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_reminder_types_modified_by",
                table: "reminder_types",
                column: "modified_by");

            migrationBuilder.AddForeignKey(
                name: "fk_reminders_reminder_types_type_id",
                table: "reminders",
                column: "type_id",
                principalTable: "reminder_types",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_reminders_reminder_types_type_id",
                table: "reminders");

            migrationBuilder.DropTable(
                name: "reminder_types");

            migrationBuilder.DropIndex(
                name: "ix_reminders_type_id",
                table: "reminders");

            migrationBuilder.DropColumn(
                name: "status",
                table: "reminders");

            migrationBuilder.RenameColumn(
                name: "type_id",
                table: "reminders",
                newName: "type");
        }
    }
}

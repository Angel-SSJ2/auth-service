using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthService.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTimestampsToAuth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_user_role_user_user_id1",
                table: "user_role");

            migrationBuilder.DropIndex(
                name: "ix_user_role_user_id",
                table: "user_role");

            migrationBuilder.DropColumn(
                name: "user_id1",
                table: "user_role");

            migrationBuilder.DropColumn(
                name: "description",
                table: "role");

            migrationBuilder.AddColumn<DateTime>(
                name: "assigned_at",
                table: "user_role",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "role",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "role",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "assigned_at",
                table: "user_role");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "role");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "role");

            migrationBuilder.AddColumn<string>(
                name: "user_id1",
                table: "user_role",
                type: "character varying(16)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "role",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "ix_user_role_user_id",
                table: "user_role",
                column: "user_id1");

            migrationBuilder.AddForeignKey(
                name: "fk_user_role_user_user_id1",
                table: "user_role",
                column: "user_id1",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

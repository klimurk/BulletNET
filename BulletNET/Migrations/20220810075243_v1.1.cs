using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BulletNET.Migrations
{
    public partial class v11 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MainRadarBoardPairing_datetime",
                table: "MainRadarBoardPairing");

            migrationBuilder.DropColumn(
                name: "boardSN",
                table: "TestGroup");

            migrationBuilder.DropColumn(
                name: "datetime",
                table: "MainRadarBoardPairing");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "boardSN",
                table: "TestGroup",
                type: "text",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "datetime",
                table: "MainRadarBoardPairing",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_MainRadarBoardPairing_datetime",
                table: "MainRadarBoardPairing",
                column: "datetime",
                unique: true);
        }
    }
}

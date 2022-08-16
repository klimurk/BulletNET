using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BulletNET.Migrations
{
    public partial class v10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "radarboardID",
                table: "TestGroup",
                type: "int(11)",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TestGroup_radarboardID",
                table: "TestGroup",
                column: "radarboardID");

            migrationBuilder.AddForeignKey(
                name: "FK_TestGroup_MainRadarBoardPairing_radarboardID",
                table: "TestGroup",
                column: "radarboardID",
                principalTable: "MainRadarBoardPairing",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestGroup_MainRadarBoardPairing_radarboardID",
                table: "TestGroup");

            migrationBuilder.DropIndex(
                name: "IX_TestGroup_radarboardID",
                table: "TestGroup");

            migrationBuilder.DropColumn(
                name: "radarboardID",
                table: "TestGroup");
        }
    }
}

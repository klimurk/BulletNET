using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BulletNET.Migrations
{
    public partial class v12 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestAction_PROG_USER_userID",
                table: "TestAction");

            migrationBuilder.DropForeignKey(
                name: "FK_TestAction_TestGroup_group_id",
                table: "TestAction");

            migrationBuilder.DropForeignKey(
                name: "FK_TestGroup_MainRadarBoardPairing_radarboardID",
                table: "TestGroup");

            migrationBuilder.DropIndex(
                name: "IX_TestAction_datetime",
                table: "TestAction");

            migrationBuilder.DropIndex(
                name: "IX_TestAction_userID",
                table: "TestAction");

            migrationBuilder.DropColumn(
                name: "LastComment",
                table: "TestAction");

            migrationBuilder.DropColumn(
                name: "datetime",
                table: "TestAction");

            migrationBuilder.DropColumn(
                name: "parameter1",
                table: "TestAction");

            migrationBuilder.DropColumn(
                name: "parameter2",
                table: "TestAction");

            migrationBuilder.DropColumn(
                name: "userID",
                table: "TestAction");

            migrationBuilder.RenameColumn(
                name: "radarboardID",
                table: "TestGroup",
                newName: "RadarBoardID");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "TestGroup",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "datetime",
                table: "TestGroup",
                newName: "TimeStamp");

            migrationBuilder.RenameIndex(
                name: "IX_TestGroup_radarboardID",
                table: "TestGroup",
                newName: "IX_TestGroup_RadarBoardID");

            migrationBuilder.RenameIndex(
                name: "IX_TestGroup_datetime",
                table: "TestGroup",
                newName: "IX_TestGroup_TimeStamp");

            migrationBuilder.RenameColumn(
                name: "valueName",
                table: "TestAction",
                newName: "ValueName");

            migrationBuilder.RenameColumn(
                name: "measured",
                table: "TestAction",
                newName: "Measured");

            migrationBuilder.RenameColumn(
                name: "maximum",
                table: "TestAction",
                newName: "Maximum");

            migrationBuilder.RenameColumn(
                name: "pass",
                table: "TestAction",
                newName: "Passed");

            migrationBuilder.RenameColumn(
                name: "minimum",
                table: "TestAction",
                newName: "Minimun");

            migrationBuilder.RenameColumn(
                name: "group_id",
                table: "TestAction",
                newName: "TestGroupID");

            migrationBuilder.RenameIndex(
                name: "IX_TestAction_group_id",
                table: "TestAction",
                newName: "IX_TestAction_TestGroupID");

            migrationBuilder.AddColumn<int>(
                name: "CommentID",
                table: "TestGroup",
                type: "int(11)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Comment = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserID = table.Column<int>(type: "int(11)", nullable: false),
                    datetime = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Comments_PROG_USER_UserID",
                        column: x => x.UserID,
                        principalTable: "PROG_USER",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_TestGroup_CommentID",
                table: "TestGroup",
                column: "CommentID");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_UserID",
                table: "Comments",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_TestAction_TestGroup_TestGroupID",
                table: "TestAction",
                column: "TestGroupID",
                principalTable: "TestGroup",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestGroup_Comments_CommentID",
                table: "TestGroup",
                column: "CommentID",
                principalTable: "Comments",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_TestGroup_MainRadarBoardPairing_RadarBoardID",
                table: "TestGroup",
                column: "RadarBoardID",
                principalTable: "MainRadarBoardPairing",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestAction_TestGroup_TestGroupID",
                table: "TestAction");

            migrationBuilder.DropForeignKey(
                name: "FK_TestGroup_Comments_CommentID",
                table: "TestGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_TestGroup_MainRadarBoardPairing_RadarBoardID",
                table: "TestGroup");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_TestGroup_CommentID",
                table: "TestGroup");

            migrationBuilder.DropColumn(
                name: "CommentID",
                table: "TestGroup");

            migrationBuilder.RenameColumn(
                name: "RadarBoardID",
                table: "TestGroup",
                newName: "radarboardID");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "TestGroup",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "TimeStamp",
                table: "TestGroup",
                newName: "datetime");

            migrationBuilder.RenameIndex(
                name: "IX_TestGroup_RadarBoardID",
                table: "TestGroup",
                newName: "IX_TestGroup_radarboardID");

            migrationBuilder.RenameIndex(
                name: "IX_TestGroup_TimeStamp",
                table: "TestGroup",
                newName: "IX_TestGroup_datetime");

            migrationBuilder.RenameColumn(
                name: "ValueName",
                table: "TestAction",
                newName: "valueName");

            migrationBuilder.RenameColumn(
                name: "Measured",
                table: "TestAction",
                newName: "measured");

            migrationBuilder.RenameColumn(
                name: "Maximum",
                table: "TestAction",
                newName: "maximum");

            migrationBuilder.RenameColumn(
                name: "Passed",
                table: "TestAction",
                newName: "pass");

            migrationBuilder.RenameColumn(
                name: "Minimun",
                table: "TestAction",
                newName: "minimum");

            migrationBuilder.RenameColumn(
                name: "TestGroupID",
                table: "TestAction",
                newName: "group_id");

            migrationBuilder.RenameIndex(
                name: "IX_TestAction_TestGroupID",
                table: "TestAction",
                newName: "IX_TestAction_group_id");

            migrationBuilder.AddColumn<string>(
                name: "LastComment",
                table: "TestAction",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "datetime",
                table: "TestAction",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "parameter1",
                table: "TestAction",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "parameter2",
                table: "TestAction",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "userID",
                table: "TestAction",
                type: "int(11)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestAction_datetime",
                table: "TestAction",
                column: "datetime",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestAction_userID",
                table: "TestAction",
                column: "userID");

            migrationBuilder.AddForeignKey(
                name: "FK_TestAction_PROG_USER_userID",
                table: "TestAction",
                column: "userID",
                principalTable: "PROG_USER",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_TestAction_TestGroup_group_id",
                table: "TestAction",
                column: "group_id",
                principalTable: "TestGroup",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestGroup_MainRadarBoardPairing_radarboardID",
                table: "TestGroup",
                column: "radarboardID",
                principalTable: "MainRadarBoardPairing",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

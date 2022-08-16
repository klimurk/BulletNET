using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BulletNET.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MainRadarBoardPairing",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    mainBoardID = table.Column<string>(type: "tinytext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    radarBoardID = table.Column<string>(type: "tinytext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    datetime = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MainRadarBoardPairing", x => x.ID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PROG_USER",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    USER_NAME = table.Column<string>(type: "varchar(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    USER_DESC = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    USER_ROLE = table.Column<int>(type: "int", nullable: false),
                    USER_HASH = table.Column<string>(type: "char(64)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PROG_USER", x => x.ID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TestGroup",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    datetime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    boardSN = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserID = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestGroup", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TestGroup_PROG_USER_UserID",
                        column: x => x.UserID,
                        principalTable: "PROG_USER",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TestAction",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    valueName = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    measured = table.Column<double>(type: "double", nullable: true),
                    maximum = table.Column<double>(type: "double", nullable: true),
                    minimum = table.Column<double>(type: "double", nullable: true),
                    pass = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    parameter1 = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    parameter2 = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    datetime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    group_id = table.Column<int>(type: "int(11)", nullable: false),
                    LastComment = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    userID = table.Column<int>(type: "int(11)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestAction", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TestAction_PROG_USER_userID",
                        column: x => x.userID,
                        principalTable: "PROG_USER",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_TestAction_TestGroup_group_id",
                        column: x => x.group_id,
                        principalTable: "TestGroup",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_MainRadarBoardPairing_datetime",
                table: "MainRadarBoardPairing",
                column: "datetime",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestAction_datetime",
                table: "TestAction",
                column: "datetime",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestAction_group_id",
                table: "TestAction",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "IX_TestAction_userID",
                table: "TestAction",
                column: "userID");

            migrationBuilder.CreateIndex(
                name: "IX_TestGroup_datetime",
                table: "TestGroup",
                column: "datetime",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestGroup_UserID",
                table: "TestGroup",
                column: "UserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MainRadarBoardPairing");

            migrationBuilder.DropTable(
                name: "TestAction");

            migrationBuilder.DropTable(
                name: "TestGroup");

            migrationBuilder.DropTable(
                name: "PROG_USER");
        }
    }
}

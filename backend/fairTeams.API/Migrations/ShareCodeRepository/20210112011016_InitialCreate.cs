using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace fairTeams.API.Migrations.ShareCodeRepository
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShareCodes",
                columns: table => new
                {
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    DownloadAttempt = table.Column<int>(type: "INTEGER", nullable: false),
                    EarliestRetry = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShareCodes", x => x.Code);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShareCodes");
        }
    }
}

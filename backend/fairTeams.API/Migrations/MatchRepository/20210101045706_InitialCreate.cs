using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace fairTeams.API.Migrations.MatchRepository
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GameRequest",
                columns: table => new
                {
                    MatchId = table.Column<ulong>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Token = table.Column<uint>(type: "INTEGER", nullable: false),
                    OutcomeId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameRequest", x => x.MatchId);
                });

            migrationBuilder.CreateTable(
                name: "Demo",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    ShareCode = table.Column<string>(type: "TEXT", nullable: true),
                    GameRequestMatchId = table.Column<ulong>(type: "INTEGER", nullable: true),
                    FilePath = table.Column<string>(type: "TEXT", nullable: true),
                    DownloadURL = table.Column<string>(type: "TEXT", nullable: true),
                    State = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Demo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Demo_GameRequest_GameRequestMatchId",
                        column: x => x.GameRequestMatchId,
                        principalTable: "GameRequest",
                        principalColumn: "MatchId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Matches",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    DemoId = table.Column<string>(type: "TEXT", nullable: true),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Map = table.Column<string>(type: "TEXT", nullable: true),
                    TScore = table.Column<int>(type: "INTEGER", nullable: false),
                    CTScore = table.Column<int>(type: "INTEGER", nullable: false),
                    Rounds = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Matches_Demo_DemoId",
                        column: x => x.DemoId,
                        principalTable: "Demo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MatchStatistics",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    SteamID = table.Column<long>(type: "INTEGER", nullable: false),
                    Kills = table.Column<int>(type: "INTEGER", nullable: false),
                    Deaths = table.Column<int>(type: "INTEGER", nullable: false),
                    Rounds = table.Column<int>(type: "INTEGER", nullable: false),
                    OneKill = table.Column<int>(type: "INTEGER", nullable: false),
                    TwoKill = table.Column<int>(type: "INTEGER", nullable: false),
                    ThreeKill = table.Column<int>(type: "INTEGER", nullable: false),
                    FourKill = table.Column<int>(type: "INTEGER", nullable: false),
                    FiveKill = table.Column<int>(type: "INTEGER", nullable: false),
                    MatchId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchStatistics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MatchStatistics_Matches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Demo_GameRequestMatchId",
                table: "Demo",
                column: "GameRequestMatchId");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_DemoId",
                table: "Matches",
                column: "DemoId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchStatistics_MatchId",
                table: "MatchStatistics",
                column: "MatchId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MatchStatistics");

            migrationBuilder.DropTable(
                name: "Matches");

            migrationBuilder.DropTable(
                name: "Demo");

            migrationBuilder.DropTable(
                name: "GameRequest");
        }
    }
}

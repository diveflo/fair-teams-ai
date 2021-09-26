using Microsoft.EntityFrameworkCore.Migrations;

namespace fairTeams.API.Migrations.MatchRepository
{
    public partial class AddMatchType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MatchType",
                table: "MatchStatistics",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MatchType",
                table: "MatchStatistics");
        }
    }
}

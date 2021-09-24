using Microsoft.EntityFrameworkCore.Migrations;

namespace fairTeams.API.Migrations.MatchRepository
{
    public partial class AddInitialSide : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InitialSide",
                table: "MatchStatistics",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InitialSide",
                table: "MatchStatistics");
        }
    }
}

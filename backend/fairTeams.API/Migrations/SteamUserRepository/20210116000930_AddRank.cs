using Microsoft.EntityFrameworkCore.Migrations;

namespace fairTeams.API.Migrations.SteamUserRepository
{
    public partial class AddRank : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Rank",
                table: "SteamUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rank",
                table: "SteamUsers");
        }
    }
}

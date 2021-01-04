using Microsoft.EntityFrameworkCore.Migrations;

namespace fairTeams.API.Migrations.SteamUserRepository
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SteamUsers",
                columns: table => new
                {
                    SteamID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AuthenticationCode = table.Column<string>(type: "TEXT", nullable: true),
                    LastSharingCode = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SteamUsers", x => x.SteamID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SteamUsers");
        }
    }
}

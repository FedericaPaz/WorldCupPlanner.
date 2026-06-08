using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Migrations
{
    /// <inheritdoc />
    public partial class FixFixture : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FixtureId",
                table: "Groups",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "GroupMatches",
                columns: table => new
                {
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    MatchId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupMatches", x => new { x.GroupId, x.MatchId });
                    table.ForeignKey(
                        name: "FK_GroupMatches_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupMatches_Matches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Matches",
                        principalColumn: "IdMatch",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupTeams",
                columns: table => new
                {
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    TeamName = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupTeams", x => new { x.GroupId, x.TeamName });
                    table.ForeignKey(
                        name: "FK_GroupTeams_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupTeams_Teams_TeamName",
                        column: x => x.TeamName,
                        principalTable: "Teams",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Groups_FixtureId",
                table: "Groups",
                column: "FixtureId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupMatches_MatchId",
                table: "GroupMatches",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupTeams_TeamName",
                table: "GroupTeams",
                column: "TeamName");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Fixtures_FixtureId",
                table: "Groups",
                column: "FixtureId",
                principalTable: "Fixtures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Fixtures_FixtureId",
                table: "Groups");

            migrationBuilder.DropTable(
                name: "GroupMatches");

            migrationBuilder.DropTable(
                name: "GroupTeams");

            migrationBuilder.DropIndex(
                name: "IX_Groups_FixtureId",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "FixtureId",
                table: "Groups");
        }
    }
}

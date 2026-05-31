using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddMatchEntityRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AwayTeamName",
                table: "Matches",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HomeTeamName",
                table: "Matches",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LoserMatchName",
                table: "Matches",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StadiumName",
                table: "Matches",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WinnerMatchName",
                table: "Matches",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Matches_AwayTeamName",
                table: "Matches",
                column: "AwayTeamName");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_HomeTeamName",
                table: "Matches",
                column: "HomeTeamName");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_LoserMatchName",
                table: "Matches",
                column: "LoserMatchName");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_StadiumName",
                table: "Matches",
                column: "StadiumName");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_WinnerMatchName",
                table: "Matches",
                column: "WinnerMatchName");

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Stadiums_StadiumName",
                table: "Matches",
                column: "StadiumName",
                principalTable: "Stadiums",
                principalColumn: "StadiumName",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Teams_AwayTeamName",
                table: "Matches",
                column: "AwayTeamName",
                principalTable: "Teams",
                principalColumn: "Name",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Teams_HomeTeamName",
                table: "Matches",
                column: "HomeTeamName",
                principalTable: "Teams",
                principalColumn: "Name",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Teams_LoserMatchName",
                table: "Matches",
                column: "LoserMatchName",
                principalTable: "Teams",
                principalColumn: "Name",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Teams_WinnerMatchName",
                table: "Matches",
                column: "WinnerMatchName",
                principalTable: "Teams",
                principalColumn: "Name",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Stadiums_StadiumName",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Teams_AwayTeamName",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Teams_HomeTeamName",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Teams_LoserMatchName",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Teams_WinnerMatchName",
                table: "Matches");

            migrationBuilder.DropIndex(
                name: "IX_Matches_AwayTeamName",
                table: "Matches");

            migrationBuilder.DropIndex(
                name: "IX_Matches_HomeTeamName",
                table: "Matches");

            migrationBuilder.DropIndex(
                name: "IX_Matches_LoserMatchName",
                table: "Matches");

            migrationBuilder.DropIndex(
                name: "IX_Matches_StadiumName",
                table: "Matches");

            migrationBuilder.DropIndex(
                name: "IX_Matches_WinnerMatchName",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "AwayTeamName",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "HomeTeamName",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "LoserMatchName",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "StadiumName",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "WinnerMatchName",
                table: "Matches");
        }
    }
}

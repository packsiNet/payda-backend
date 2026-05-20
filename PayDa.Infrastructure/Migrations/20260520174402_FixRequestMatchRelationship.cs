using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayDa.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixRequestMatchRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Matches_MatchId",
                table: "Requests");

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_Matches_MatchId",
                table: "Requests",
                column: "MatchId",
                principalTable: "Matches",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Matches_MatchId",
                table: "Requests");

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_Matches_MatchId",
                table: "Requests",
                column: "MatchId",
                principalTable: "Matches",
                principalColumn: "Id");
        }
    }
}

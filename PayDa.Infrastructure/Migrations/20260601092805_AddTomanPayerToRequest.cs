using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayDa.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTomanPayerToRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TomanPayerFullName",
                table: "Requests",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TomanPayerMobileNumber",
                table: "Requests",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TomanPayerFullName",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "TomanPayerMobileNumber",
                table: "Requests");
        }
    }
}

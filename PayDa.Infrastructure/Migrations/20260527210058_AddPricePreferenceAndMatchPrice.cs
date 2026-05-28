using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayDa.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPricePreferenceAndMatchPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommissionAmount",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "CommissionPercent",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "RateValue",
                table: "Requests");

            migrationBuilder.RenameColumn(
                name: "RateType",
                table: "Requests",
                newName: "PricePreference");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Matches",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "Matches");

            migrationBuilder.RenameColumn(
                name: "PricePreference",
                table: "Requests",
                newName: "RateType");

            migrationBuilder.AddColumn<decimal>(
                name: "CommissionAmount",
                table: "Requests",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CommissionPercent",
                table: "Requests",
                type: "numeric(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "RateValue",
                table: "Requests",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}

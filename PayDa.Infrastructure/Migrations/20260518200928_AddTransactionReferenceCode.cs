using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayDa.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTransactionReferenceCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReferenceCode",
                table: "Transactions",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_ReferenceCode",
                table: "Transactions",
                column: "ReferenceCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Transactions_ReferenceCode",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "ReferenceCode",
                table: "Transactions");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayDa.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefactorTransactionFlow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ScreenshotUrl",
                table: "Transactions");

            migrationBuilder.RenameColumn(
                name: "PaidAt",
                table: "Transactions",
                newName: "TomanDeclaredAt");

            migrationBuilder.RenameColumn(
                name: "ConfirmedAt",
                table: "Transactions",
                newName: "TomanConfirmedAt");

            migrationBuilder.RenameColumn(
                name: "SettledAt",
                table: "Transactions",
                newName: "CompletedAt");

            migrationBuilder.AddColumn<string>(
                name: "ForeignReceiptUrl",
                table: "Transactions",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ForeignTransferredAt",
                table: "Transactions",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ForeignReceiptUrl",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "ForeignTransferredAt",
                table: "Transactions");

            migrationBuilder.RenameColumn(
                name: "TomanDeclaredAt",
                table: "Transactions",
                newName: "PaidAt");

            migrationBuilder.RenameColumn(
                name: "TomanConfirmedAt",
                table: "Transactions",
                newName: "ConfirmedAt");

            migrationBuilder.RenameColumn(
                name: "CompletedAt",
                table: "Transactions",
                newName: "SettledAt");

            migrationBuilder.AddColumn<string>(
                name: "ScreenshotUrl",
                table: "Transactions",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);
        }
    }
}

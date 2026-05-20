using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayDa.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRequestForeignAccounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "ReceiverId",
                table: "Requests",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.CreateTable(
                name: "RequestForeignAccounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestId = table.Column<Guid>(type: "uuid", nullable: false),
                    Method = table.Column<int>(type: "integer", nullable: false),
                    FullName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    EmailOrPhone = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Iban = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Bic = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    BankName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    AccountNum = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Swift = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    BankAddress = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestForeignAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestForeignAccounts_Requests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "Requests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RequestForeignAccounts_RequestId",
                table: "RequestForeignAccounts",
                column: "RequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequestForeignAccounts");

            migrationBuilder.AlterColumn<Guid>(
                name: "ReceiverId",
                table: "Requests",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);
        }
    }
}

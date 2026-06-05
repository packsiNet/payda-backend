using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayDa.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReferralSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                ALTER TABLE "Users" ADD COLUMN IF NOT EXISTS "ReferralCode" character varying(8) NOT NULL DEFAULT '';
                ALTER TABLE "Users" ADD COLUMN IF NOT EXISTS "ReferredById" uuid NULL;

                CREATE UNIQUE INDEX IF NOT EXISTS "IX_Users_ReferralCode" ON "Users" ("ReferralCode");
                CREATE INDEX IF NOT EXISTS "IX_Users_ReferredById" ON "Users" ("ReferredById");

                DO $$ BEGIN
                    IF NOT EXISTS (
                        SELECT 1 FROM pg_constraint WHERE conname = 'FK_Users_Users_ReferredById'
                    ) THEN
                        ALTER TABLE "Users"
                            ADD CONSTRAINT "FK_Users_Users_ReferredById"
                            FOREIGN KEY ("ReferredById") REFERENCES "Users" ("Id")
                            ON DELETE SET NULL;
                    END IF;
                END $$;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Users_ReferredById",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_ReferralCode",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_ReferredById",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ReferralCode",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ReferredById",
                table: "Users");
        }
    }
}

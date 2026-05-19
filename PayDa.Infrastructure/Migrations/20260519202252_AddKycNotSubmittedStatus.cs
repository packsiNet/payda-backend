using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayDa.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddKycNotSubmittedStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Shift existing values: Rejected 2→3, Approved 1→2
            migrationBuilder.Sql("UPDATE \"Users\" SET \"KycStatus\" = 3 WHERE \"KycStatus\" = 2;");
            migrationBuilder.Sql("UPDATE \"Users\" SET \"KycStatus\" = 2 WHERE \"KycStatus\" = 1;");
            // Users who actually submitted KYC (old Pending=0 with KycSubmittedAt set) → new Pending=1
            migrationBuilder.Sql("UPDATE \"Users\" SET \"KycStatus\" = 1 WHERE \"KycStatus\" = 0 AND \"KycSubmittedAt\" IS NOT NULL;");
            // Remaining 0 values stay as 0 = NotSubmitted (correct)
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Reverse: Pending 1→0 where submitted, Approved 2→1, Rejected 3→2
            migrationBuilder.Sql("UPDATE \"Users\" SET \"KycStatus\" = 0 WHERE \"KycStatus\" = 1 AND \"KycSubmittedAt\" IS NOT NULL;");
            migrationBuilder.Sql("UPDATE \"Users\" SET \"KycStatus\" = 1 WHERE \"KycStatus\" = 2;");
            migrationBuilder.Sql("UPDATE \"Users\" SET \"KycStatus\" = 2 WHERE \"KycStatus\" = 3;");
        }
    }
}

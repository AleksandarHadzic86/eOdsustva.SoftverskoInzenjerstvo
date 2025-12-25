using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eOdsustva.SoftverskoInzenjerstvo.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedinExtendUsesrTableUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0bff17a4-8768-48d7-a9ad-962a8ad7b7a0",
                columns: new[] { "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "ROLE_SUPERVISOR_001", "Supervisor", "SUPERVISOR" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0bff17a4-8768-48d7-a9ad-962a8ad7b7a0",
                columns: new[] { "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "ROLE_SUPERVISIOR_001", "Supervisior", "SUPERVISIOR" });
        }
    }
}

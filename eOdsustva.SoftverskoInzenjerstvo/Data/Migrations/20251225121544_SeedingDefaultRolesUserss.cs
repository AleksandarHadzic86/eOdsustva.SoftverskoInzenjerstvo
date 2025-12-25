using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eOdsustva.SoftverskoInzenjerstvo.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedingDefaultRolesUserss : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0bff17a4-8768-48d7-a9ad-962a8ad7b7a0",
                column: "ConcurrencyStamp",
                value: "ROLE_SUPERVISIOR_001");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "42715495-8f6e-4625-89b3-8a1f76f7e274",
                column: "ConcurrencyStamp",
                value: "ROLE_EMPLOYEE_001");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b748e02c-457d-4326-ac2f-6d2868ed9337",
                column: "ConcurrencyStamp",
                value: "ROLE_ADMIN_001");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0bff17a4-8768-48d7-a9ad-962a8ad7b7a0",
                column: "ConcurrencyStamp",
                value: "60481265-7016-48c5-811a-f367f31b0c05");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "42715495-8f6e-4625-89b3-8a1f76f7e274",
                column: "ConcurrencyStamp",
                value: "a1e479f9-ae60-4b33-bf2a-639f1769cadc");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b748e02c-457d-4326-ac2f-6d2868ed9337",
                column: "ConcurrencyStamp",
                value: "c80dce23-ce4c-45fb-9ebc-bd7e6f9551f2");
        }
    }
}

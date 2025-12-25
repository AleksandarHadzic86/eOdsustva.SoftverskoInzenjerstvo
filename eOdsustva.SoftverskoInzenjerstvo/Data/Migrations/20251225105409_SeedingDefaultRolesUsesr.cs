using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace eOdsustva.SoftverskoInzenjerstvo.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedingDefaultRolesUsesr : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "0bff17a4-8768-48d7-a9ad-962a8ad7b7a0", "3f29159d-671f-46cb-8693-0773b989f4dd", "Supervisior", "SUPERVISIOR" },
                    { "42715495-8f6e-4625-89b3-8a1f76f7e274", "c2771c8b-3880-4131-a785-328ec406c4d8", "Employee", "EMPLOYEE" },
                    { "b748e02c-457d-4326-ac2f-6d2868ed9337", "5074593c-0a8f-46a4-b624-e8630a8d66c5", "Administrator", "ADMINISTRATOR" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "c8f2b1d3-3f4e-4d5a-9c3a-1e2b3c4d5e6f", 0, "619ced8a-2430-491f-8361-ba1564a7cdfa", "aleksandarhadzic1986@gmail.com", true, false, null, "ALEKSANDARHADZIC1986@GMAIL.COM", "ALEKSANDARHADZIC1986@GMAIL.COM", "AQAAAAIAAYagAAAAEA9awdL2JN5/tLZ4/566Cvd4dn0SsxhIKNMLKgOE2foyG3Coo823E9wEBfJO4s4FZw==", null, false, "d7307023-91eb-442a-b980-3a6b102ff83f", false, "aleksandarhadzic1986@gmail.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "b748e02c-457d-4326-ac2f-6d2868ed9337", "c8f2b1d3-3f4e-4d5a-9c3a-1e2b3c4d5e6f" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0bff17a4-8768-48d7-a9ad-962a8ad7b7a0");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "42715495-8f6e-4625-89b3-8a1f76f7e274");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "b748e02c-457d-4326-ac2f-6d2868ed9337", "c8f2b1d3-3f4e-4d5a-9c3a-1e2b3c4d5e6f" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b748e02c-457d-4326-ac2f-6d2868ed9337");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "c8f2b1d3-3f4e-4d5a-9c3a-1e2b3c4d5e6f");
        }
    }
}

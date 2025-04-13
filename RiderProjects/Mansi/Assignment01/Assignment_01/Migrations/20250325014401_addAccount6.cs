using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Assignment01.Migrations
{
    /// <inheritdoc />
    public partial class addAccount6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8903e3be-d38a-4885-b2b6-f24463ac4147");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8a0b87e8-efae-466a-9b24-35f33c768556");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "94610ca4-539c-4ce2-bd33-945ce2fd551d");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "8903e3be-d38a-4885-b2b6-f24463ac4147", null, "Admin", "admin" },
                    { "8a0b87e8-efae-466a-9b24-35f33c768556", null, "client", "client" },
                    { "94610ca4-539c-4ce2-bd33-945ce2fd551d", null, "seller", "seller" }
                });
        }
    }
}

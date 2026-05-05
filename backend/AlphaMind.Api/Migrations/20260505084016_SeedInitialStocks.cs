using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AlphaMind.Api.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialStocks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Stocks",
                columns: new[] { "Id", "CreatedAt", "Exchange", "IsActive", "Name", "Ticker" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 5, 5, 0, 0, 0, 0, DateTimeKind.Utc), "US", true, "Apple Inc.", "AAPL" },
                    { 2, new DateTime(2026, 5, 5, 0, 0, 0, 0, DateTimeKind.Utc), "US", true, "Microsoft Corporation", "MSFT" },
                    { 3, new DateTime(2026, 5, 5, 0, 0, 0, 0, DateTimeKind.Utc), "US", true, "NVIDIA Corporation", "NVDA" },
                    { 4, new DateTime(2026, 5, 5, 0, 0, 0, 0, DateTimeKind.Utc), "US", true, "Advanced Micro Devices, Inc.", "AMD" },
                    { 5, new DateTime(2026, 5, 5, 0, 0, 0, 0, DateTimeKind.Utc), "Stockholm", true, "Ericsson B", "ERIC-B.ST" },
                    { 6, new DateTime(2026, 5, 5, 0, 0, 0, 0, DateTimeKind.Utc), "Stockholm", true, "Volvo B", "VOLV-B.ST" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: 6);
        }
    }
}

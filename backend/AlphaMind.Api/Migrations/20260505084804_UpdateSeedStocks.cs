using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AlphaMind.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeedStocks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Exchange", "Name", "Ticker" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Stockholm", "Investor A", "INVE-A.ST" });

            migrationBuilder.UpdateData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "Name", "Ticker" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Alphabet Inc Class C", "GOOG" });

            migrationBuilder.UpdateData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "Name", "Ticker" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Microsoft", "MSFT" });

            migrationBuilder.UpdateData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "Name", "Ticker" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Taiwan Semiconductor Mfg Co", "TSM" });

            migrationBuilder.UpdateData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "Name", "Ticker" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Industrivärden C", "INDU-C.ST" });

            migrationBuilder.UpdateData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedAt", "Exchange", "Name", "Ticker" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Netherlands", "ASML HOLDING", "ASML" });

            migrationBuilder.InsertData(
                table: "Stocks",
                columns: new[] { "Id", "CreatedAt", "Exchange", "IsActive", "Name", "Ticker" },
                values: new object[,]
                {
                    { 7, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "US", true, "NVIDIA", "NVDA" },
                    { 8, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Stockholm", true, "Svolder B", "SVOL-B.ST" },
                    { 9, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "US", true, "Advanced Micro Devices", "AMD" },
                    { 10, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "US", true, "Amazon.com", "AMZN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.UpdateData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Exchange", "Name", "Ticker" },
                values: new object[] { new DateTime(2026, 5, 5, 0, 0, 0, 0, DateTimeKind.Utc), "US", "Apple Inc.", "AAPL" });

            migrationBuilder.UpdateData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "Name", "Ticker" },
                values: new object[] { new DateTime(2026, 5, 5, 0, 0, 0, 0, DateTimeKind.Utc), "Microsoft Corporation", "MSFT" });

            migrationBuilder.UpdateData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "Name", "Ticker" },
                values: new object[] { new DateTime(2026, 5, 5, 0, 0, 0, 0, DateTimeKind.Utc), "NVIDIA Corporation", "NVDA" });

            migrationBuilder.UpdateData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "Name", "Ticker" },
                values: new object[] { new DateTime(2026, 5, 5, 0, 0, 0, 0, DateTimeKind.Utc), "Advanced Micro Devices, Inc.", "AMD" });

            migrationBuilder.UpdateData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "Name", "Ticker" },
                values: new object[] { new DateTime(2026, 5, 5, 0, 0, 0, 0, DateTimeKind.Utc), "Ericsson B", "ERIC-B.ST" });

            migrationBuilder.UpdateData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedAt", "Exchange", "Name", "Ticker" },
                values: new object[] { new DateTime(2026, 5, 5, 0, 0, 0, 0, DateTimeKind.Utc), "Stockholm", "Volvo B", "VOLV-B.ST" });
        }
    }
}

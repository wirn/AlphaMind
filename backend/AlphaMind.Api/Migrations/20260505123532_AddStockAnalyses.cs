using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlphaMind.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddStockAnalyses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "Stocks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "StockAnalyses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StockId = table.Column<int>(type: "int", nullable: false),
                    ImpactScore = table.Column<int>(type: "int", nullable: false),
                    ConfidenceScore = table.Column<int>(type: "int", nullable: false),
                    ExpectedMove = table.Column<int>(type: "int", nullable: false),
                    Direction = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Summary = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    OpportunitiesJson = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: false),
                    RisksJson = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: false),
                    UsedNewsCount = table.Column<int>(type: "int", nullable: false),
                    AnalyzedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockAnalyses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockAnalyses_Stocks_StockId",
                        column: x => x.StockId,
                        principalTable: "Stocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: 1,
                column: "SortOrder",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: 2,
                column: "SortOrder",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: 3,
                column: "SortOrder",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: 4,
                column: "SortOrder",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: 5,
                column: "SortOrder",
                value: 5);

            migrationBuilder.UpdateData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: 6,
                column: "SortOrder",
                value: 6);

            migrationBuilder.UpdateData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: 7,
                column: "SortOrder",
                value: 7);

            migrationBuilder.UpdateData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: 8,
                column: "SortOrder",
                value: 8);

            migrationBuilder.UpdateData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: 9,
                column: "SortOrder",
                value: 9);

            migrationBuilder.UpdateData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: 10,
                column: "SortOrder",
                value: 10);

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_SortOrder",
                table: "Stocks",
                column: "SortOrder");

            migrationBuilder.CreateIndex(
                name: "IX_StockAnalyses_AnalyzedAt",
                table: "StockAnalyses",
                column: "AnalyzedAt");

            migrationBuilder.CreateIndex(
                name: "IX_StockAnalyses_ImpactScore",
                table: "StockAnalyses",
                column: "ImpactScore");

            migrationBuilder.CreateIndex(
                name: "IX_StockAnalyses_StockId",
                table: "StockAnalyses",
                column: "StockId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StockAnalyses");

            migrationBuilder.DropIndex(
                name: "IX_Stocks_SortOrder",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "Stocks");
        }
    }
}

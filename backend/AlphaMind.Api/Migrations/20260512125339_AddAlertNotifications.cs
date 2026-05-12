using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlphaMind.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddAlertNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AlertNotifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StockAnalysisId = table.Column<int>(type: "int", nullable: false),
                    StockId = table.Column<int>(type: "int", nullable: false),
                    Ticker = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    RecipientEmail = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                    ImpactScore = table.Column<int>(type: "int", nullable: false),
                    Direction = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SentAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlertNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AlertNotifications_StockAnalyses_StockAnalysisId",
                        column: x => x.StockAnalysisId,
                        principalTable: "StockAnalyses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlertNotifications_CreatedAtUtc",
                table: "AlertNotifications",
                column: "CreatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_AlertNotifications_StockAnalysisId",
                table: "AlertNotifications",
                column: "StockAnalysisId");

            migrationBuilder.CreateIndex(
                name: "IX_AlertNotifications_StockAnalysisId_RecipientEmail",
                table: "AlertNotifications",
                columns: new[] { "StockAnalysisId", "RecipientEmail" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AlertNotifications_StockId",
                table: "AlertNotifications",
                column: "StockId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlertNotifications");
        }
    }
}

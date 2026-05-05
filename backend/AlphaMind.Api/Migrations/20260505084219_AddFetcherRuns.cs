using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlphaMind.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddFetcherRuns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FetcherRuns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FinishedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    StocksChecked = table.Column<int>(type: "int", nullable: false),
                    NewsFetched = table.Column<int>(type: "int", nullable: false),
                    NewNewsSaved = table.Column<int>(type: "int", nullable: false),
                    DuplicatesSkipped = table.Column<int>(type: "int", nullable: false),
                    ErrorsCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FetcherRuns", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FetcherRuns");
        }
    }
}

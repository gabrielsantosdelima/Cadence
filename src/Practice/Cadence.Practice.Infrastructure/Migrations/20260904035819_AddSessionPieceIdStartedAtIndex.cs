using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cadence.Practice.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSessionPieceIdStartedAtIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Sessions_PieceId_StartedAtUtc",
                table: "Sessions",
                columns: new[] { "PieceId", "StartedAtUtc" },
                descending: new[] { false, true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Sessions_PieceId_StartedAtUtc",
                table: "Sessions");
        }
    }
}

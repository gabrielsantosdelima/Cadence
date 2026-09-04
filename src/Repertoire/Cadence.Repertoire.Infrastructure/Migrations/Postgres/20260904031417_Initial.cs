using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cadence.Repertoire.Infrastructure.Migrations.Postgres
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Pieces",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Composer = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    Genre = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Difficulty = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Key = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    ReferenceUrl = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AverageQuality = table.Column<decimal>(type: "numeric", nullable: true),
                    LastPracticedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SessionCount = table.Column<int>(type: "integer", nullable: false),
                    TotalMinutes = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pieces", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProcessedMessages",
                columns: table => new
                {
                    MessageId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConsumerName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ProcessedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessedMessages", x => x.MessageId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pieces_Genre",
                table: "Pieces",
                column: "Genre");

            migrationBuilder.CreateIndex(
                name: "IX_Pieces_Status",
                table: "Pieces",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessedMessages_MessageId_ConsumerName",
                table: "ProcessedMessages",
                columns: new[] { "MessageId", "ConsumerName" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pieces");

            migrationBuilder.DropTable(
                name: "ProcessedMessages");
        }
    }
}

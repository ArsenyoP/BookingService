using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class reviews_indexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Reviews_Score",
                table: "Reviews",
                column: "Score");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_TargetId_CreatedAt",
                table: "Reviews",
                columns: new[] { "TargetId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_UserId",
                table: "Reviews",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reviews_Score",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_TargetId_CreatedAt",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_UserId",
                table: "Reviews");
        }
    }
}

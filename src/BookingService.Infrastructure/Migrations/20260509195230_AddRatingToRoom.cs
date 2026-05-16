using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRatingToRoom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AverageRating",
                table: "Rooms",
                type: "decimal(3,2)",
                nullable: false,
                defaultValue: 0.0m);

            migrationBuilder.AddColumn<int>(
                name: "ReviewsCount",
                table: "Rooms",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AverageRating",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "ReviewsCount",
                table: "Rooms");
        }
    }
}

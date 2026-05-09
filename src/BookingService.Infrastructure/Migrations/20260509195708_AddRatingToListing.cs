using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRatingToListing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AverageRating",
                table: "Listings",
                type: "decimal(3,2)",
                nullable: false,
                defaultValue: 0.0m);

            migrationBuilder.AddColumn<int>(
                name: "ReviewsCount",
                table: "Listings",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AverageRating",
                table: "Listings");

            migrationBuilder.DropColumn(
                name: "ReviewsCount",
                table: "Listings");
        }
    }
}

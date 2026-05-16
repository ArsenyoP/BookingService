using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTargetTypeConstrains : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK+Review_Text_Range",
                table: "Reviews");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Review_TargetType",
                table: "Reviews",
                sql: "[TargetType] IN ('Room', 'Listing')");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Review_Text_Range",
                table: "Reviews",
                sql: "LEN([Text]) > 10 AND LEN([Text]) < 1000");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Review_TargetType",
                table: "Reviews");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Review_Text_Range",
                table: "Reviews");

            migrationBuilder.AddCheckConstraint(
                name: "CK+Review_Text_Range",
                table: "Reviews",
                sql: "LEN([Text]) > 10 AND LEN([Text]) < 1000");
        }
    }
}

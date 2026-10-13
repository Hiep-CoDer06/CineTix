using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineTix.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueSeatCodePerShowtime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Seats_ShowtimeId",
                table: "Seats");

            migrationBuilder.CreateIndex(
                name: "IX_Seats_ShowtimeId_SeatCode",
                table: "Seats",
                columns: new[] { "ShowtimeId", "SeatCode" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Seats_ShowtimeId_SeatCode",
                table: "Seats");

            migrationBuilder.CreateIndex(
                name: "IX_Seats_ShowtimeId",
                table: "Seats",
                column: "ShowtimeId");
        }
    }
}

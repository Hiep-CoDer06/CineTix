using CineTix.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineTix.Migrations;

[DbContext(typeof(CineTixDbContext))]
[Migration("20261009200000_AddFoodCombos")]
public partial class AddFoodCombos : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "FoodCombos",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                IsAvailable = table.Column<bool>(type: "bit", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_FoodCombos", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "BookingCombos",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                BookingId = table.Column<int>(type: "int", nullable: false),
                FoodComboId = table.Column<int>(type: "int", nullable: false),
                ComboName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Quantity = table.Column<int>(type: "int", nullable: false),
                UnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_BookingCombos", x => x.Id);
                table.ForeignKey(
                    name: "FK_BookingCombos_Bookings_BookingId",
                    column: x => x.BookingId,
                    principalTable: "Bookings",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_BookingCombos_FoodCombos_FoodComboId",
                    column: x => x.FoodComboId,
                    principalTable: "FoodCombos",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_BookingCombos_BookingId_FoodComboId",
            table: "BookingCombos",
            columns: new[] { "BookingId", "FoodComboId" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_BookingCombos_FoodComboId",
            table: "BookingCombos",
            column: "FoodComboId");

        migrationBuilder.InsertData(
            table: "FoodCombos",
            columns: new[] { "Id", "Name", "Description", "Price", "IsAvailable" },
            values: new object[,]
            {
                { 1, "Combo Cine Solo", "1 bắp rang vừa và 1 nước ngọt 500ml.", 69000m, true },
                { 2, "Combo Cine Couple", "1 bắp rang lớn và 2 nước ngọt 500ml.", 119000m, true },
                { 3, "Combo Cine Family", "1 bắp rang lớn, 4 nước ngọt và 2 xúc xích.", 199000m, true },
                { 4, "Combo Bắp Phô Mai", "1 bắp rang phô mai lớn và 1 nước ngọt 500ml.", 79000m, true }
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "BookingCombos");
        migrationBuilder.DropTable(name: "FoodCombos");
    }
}

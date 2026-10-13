using CineTix.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineTix.Migrations;

[DbContext(typeof(CineTixDbContext))]
[Migration("20261009190000_AddMovieFeaturedStatus")]
public partial class AddMovieFeaturedStatus : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "IsFeatured",
            table: "Movies",
            type: "bit",
            nullable: false,
            defaultValue: false);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "IsFeatured",
            table: "Movies");
    }
}

using CineTix.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineTix.Migrations;

[DbContext(typeof(CineTixDbContext))]
[Migration("20261009170000_AddMovieHotStatus")]
public partial class AddMovieHotStatus : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "IsHot",
            table: "Movies",
            type: "bit",
            nullable: false,
            defaultValue: false);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "IsHot",
            table: "Movies");
    }
}

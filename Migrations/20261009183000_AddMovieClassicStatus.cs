using CineTix.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CineTix.Migrations;

[DbContext(typeof(CineTixDbContext))]
[Migration("20261009183000_AddMovieClassicStatus")]
public partial class AddMovieClassicStatus : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "IsClassic",
            table: "Movies",
            type: "bit",
            nullable: false,
            defaultValue: false);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "IsClassic",
            table: "Movies");
    }
}

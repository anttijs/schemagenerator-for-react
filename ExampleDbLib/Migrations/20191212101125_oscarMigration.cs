using Microsoft.EntityFrameworkCore.Migrations;

namespace ExampleDbLib.Migrations
{
    public partial class oscarMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "OscarWinner",
                table: "Movies",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OscarWinner",
                table: "Movies");
        }
    }
}

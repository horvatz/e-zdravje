using Microsoft.EntityFrameworkCore.Migrations;

namespace eZdravje.Migrations
{
    public partial class auth3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Specialists",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Specialists");
        }
    }
}

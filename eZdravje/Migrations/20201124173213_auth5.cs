using Microsoft.EntityFrameworkCore.Migrations;

namespace eZdravje.Migrations
{
    public partial class auth5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Specialists");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Patients");

            migrationBuilder.AddColumn<string>(
                name: "UsersId",
                table: "Specialists",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsersId",
                table: "Patients",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsersId",
                table: "Specialists");

            migrationBuilder.DropColumn(
                name: "UsersId",
                table: "Patients");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Specialists",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}

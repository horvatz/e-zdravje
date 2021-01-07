using Microsoft.EntityFrameworkCore.Migrations;

namespace eZdravje.Migrations
{
    public partial class LoginUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Specialists");

            migrationBuilder.DropColumn(
                name: "UsersId",
                table: "Specialists");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "UsersId",
                table: "Patients");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Specialists",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Patients",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Specialists");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Patients");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Specialists",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsersId",
                table: "Specialists",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsersId",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}

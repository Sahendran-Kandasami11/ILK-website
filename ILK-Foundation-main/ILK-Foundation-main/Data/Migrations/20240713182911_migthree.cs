using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace test_ngo.Data.Migrations
{
    /// <inheritdoc />
    public partial class migthree : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VolunteerEmail",
                table: "Volunteer",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VolunteerSurname",
                table: "Volunteer",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "volunteerName",
                table: "Volunteer",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VolunteerEmail",
                table: "Volunteer");

            migrationBuilder.DropColumn(
                name: "VolunteerSurname",
                table: "Volunteer");

            migrationBuilder.DropColumn(
                name: "volunteerName",
                table: "Volunteer");
        }
    }
}

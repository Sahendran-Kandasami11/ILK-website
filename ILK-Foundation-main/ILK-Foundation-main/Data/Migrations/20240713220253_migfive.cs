using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace test_ngo.Data.Migrations
{
    /// <inheritdoc />
    public partial class migfive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "volunteerName",
                table: "Volunteer",
                newName: "VolunteerName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VolunteerName",
                table: "Volunteer",
                newName: "volunteerName");
        }
    }
}

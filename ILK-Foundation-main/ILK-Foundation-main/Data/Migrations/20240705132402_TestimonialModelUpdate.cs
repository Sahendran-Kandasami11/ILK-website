using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace test_ngo.Data.Migrations
{
    /// <inheritdoc />
    public partial class TestimonialModelUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Testimonials");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Testimonials",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}

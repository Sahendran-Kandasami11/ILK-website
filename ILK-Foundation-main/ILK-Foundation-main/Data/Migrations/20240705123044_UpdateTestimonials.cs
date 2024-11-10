using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace test_ngo.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTestimonials : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "ImageData",
                table: "Testimonials",
                type: "varbinary(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageData",
                table: "Testimonials");
        }
    }
}

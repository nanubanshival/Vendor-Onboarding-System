using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VendorOnboardingSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddChangesInEDIT : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EditRequestMessage",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "HasEditRequest",
                table: "Vendors");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EditRequestMessage",
                table: "Vendors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasEditRequest",
                table: "Vendors",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}

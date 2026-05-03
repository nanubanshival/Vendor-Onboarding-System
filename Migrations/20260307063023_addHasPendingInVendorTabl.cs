using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VendorOnboardingSystem.Migrations
{
    /// <inheritdoc />
    public partial class addHasPendingInVendorTabl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasPendingEditRequest",
                table: "Vendors",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasPendingEditRequest",
                table: "Vendors");
        }
    }
}

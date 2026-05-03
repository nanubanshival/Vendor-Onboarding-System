using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VendorOnboardingSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddVendorLoginFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsLockedOut",
                table: "Vendors",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLoginAt",
                table: "Vendors",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LoginAttempts",
                table: "Vendors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Vendors",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsLockedOut",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "LastLoginAt",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "LoginAttempts",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Vendors");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VendorOnboardingSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddEditRequestSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EditRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VendorId = table.Column<int>(type: "int", nullable: false),
                    AdminId = table.Column<int>(type: "int", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdminNotes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VendorResponseDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FieldsToUpdate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FieldComments = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EditRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EditRequests_AdminUsers_AdminId",
                        column: x => x.AdminId,
                        principalTable: "AdminUsers",
                        principalColumn: "AdminUserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EditRequests_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "VendorId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VendorEdits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EditRequestId = table.Column<int>(type: "int", nullable: false),
                    SubmittedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedData = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdminResponse = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorEdits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VendorEdits_EditRequests_EditRequestId",
                        column: x => x.EditRequestId,
                        principalTable: "EditRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EditRequests_AdminId",
                table: "EditRequests",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_EditRequests_VendorId",
                table: "EditRequests",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorEdits_EditRequestId",
                table: "VendorEdits",
                column: "EditRequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VendorEdits");

            migrationBuilder.DropTable(
                name: "EditRequests");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OMMS.DAL.Migrations
{
    /// <inheritdoc />
    public partial class BranchPropProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BranchId",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Products_BranchId",
                table: "Products",
                column: "BranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Branchs_BranchId",
                table: "Products",
                column: "BranchId",
                principalTable: "Branchs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Branchs_BranchId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_BranchId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "Products");
        }
    }
}

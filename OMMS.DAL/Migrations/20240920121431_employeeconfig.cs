using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OMMS.DAL.Migrations
{
    /// <inheritdoc />
    public partial class employeeconfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Branchs_BranchId",
                table: "Employees");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Branchs_BranchId",
                table: "Employees",
                column: "BranchId",
                principalTable: "Branchs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Branchs_BranchId",
                table: "Employees");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Branchs_BranchId",
                table: "Employees",
                column: "BranchId",
                principalTable: "Branchs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

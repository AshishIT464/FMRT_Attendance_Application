using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FMRT.Attendance.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHalfDayLeave : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsHalfDay",
                table: "EmployeeLeaves",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsHalfDay",
                table: "EmployeeLeaves");
        }
    }
}

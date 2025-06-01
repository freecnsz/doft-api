using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace doft.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Setup5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RepeatId",
                table: "DoftTasks",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RepeatId",
                table: "DoftTasks");
        }
    }
}

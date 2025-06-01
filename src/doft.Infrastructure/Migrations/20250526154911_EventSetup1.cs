using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace doft.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EventSetup1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EventDate",
                table: "Events",
                newName: "ToDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "FromDate",
                table: "Events",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "RepeatId",
                table: "Events",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FromDate",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "RepeatId",
                table: "Events");

            migrationBuilder.RenameColumn(
                name: "ToDate",
                table: "Events",
                newName: "EventDate");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace doft.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Setup3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TagLinks_DoftTasks_DoftTaskId",
                table: "TagLinks");

            migrationBuilder.DropForeignKey(
                name: "FK_TagLinks_Events_EventId",
                table: "TagLinks");

            migrationBuilder.DropForeignKey(
                name: "FK_TagLinks_Notes_NoteId",
                table: "TagLinks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_DoftTasks_DoftTaskId",
                table: "Tags");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Events_EventId",
                table: "Tags");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Notes_NoteId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_DoftTaskId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_EventId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_NoteId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_TagLinks_DoftTaskId",
                table: "TagLinks");

            migrationBuilder.DropIndex(
                name: "IX_TagLinks_EventId",
                table: "TagLinks");

            migrationBuilder.DropIndex(
                name: "IX_TagLinks_NoteId",
                table: "TagLinks");

            migrationBuilder.DropColumn(
                name: "DoftTaskId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "NoteId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "DoftTaskId",
                table: "TagLinks");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "TagLinks");

            migrationBuilder.DropColumn(
                name: "NoteId",
                table: "TagLinks");

            migrationBuilder.CreateIndex(
                name: "IX_TagLinks_ItemId",
                table: "TagLinks",
                column: "ItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_TagLinks_DoftTasks",
                table: "TagLinks",
                column: "ItemId",
                principalTable: "DoftTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TagLinks_Events",
                table: "TagLinks",
                column: "ItemId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TagLinks_Notes",
                table: "TagLinks",
                column: "ItemId",
                principalTable: "Notes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TagLinks_DoftTasks",
                table: "TagLinks");

            migrationBuilder.DropForeignKey(
                name: "FK_TagLinks_Events",
                table: "TagLinks");

            migrationBuilder.DropForeignKey(
                name: "FK_TagLinks_Notes",
                table: "TagLinks");

            migrationBuilder.DropIndex(
                name: "IX_TagLinks_ItemId",
                table: "TagLinks");

            migrationBuilder.AddColumn<int>(
                name: "DoftTaskId",
                table: "Tags",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EventId",
                table: "Tags",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NoteId",
                table: "Tags",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DoftTaskId",
                table: "TagLinks",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EventId",
                table: "TagLinks",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NoteId",
                table: "TagLinks",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_DoftTaskId",
                table: "Tags",
                column: "DoftTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_EventId",
                table: "Tags",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_NoteId",
                table: "Tags",
                column: "NoteId");

            migrationBuilder.CreateIndex(
                name: "IX_TagLinks_DoftTaskId",
                table: "TagLinks",
                column: "DoftTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_TagLinks_EventId",
                table: "TagLinks",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_TagLinks_NoteId",
                table: "TagLinks",
                column: "NoteId");

            migrationBuilder.AddForeignKey(
                name: "FK_TagLinks_DoftTasks_DoftTaskId",
                table: "TagLinks",
                column: "DoftTaskId",
                principalTable: "DoftTasks",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TagLinks_Events_EventId",
                table: "TagLinks",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TagLinks_Notes_NoteId",
                table: "TagLinks",
                column: "NoteId",
                principalTable: "Notes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_DoftTasks_DoftTaskId",
                table: "Tags",
                column: "DoftTaskId",
                principalTable: "DoftTasks",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Events_EventId",
                table: "Tags",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Notes_NoteId",
                table: "Tags",
                column: "NoteId",
                principalTable: "Notes",
                principalColumn: "Id");
        }
    }
}

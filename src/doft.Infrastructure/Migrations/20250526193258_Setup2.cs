using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace doft.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Setup2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_AspNetUsers_OwnerId",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_DoftTasks_Categories_CategoryId",
                table: "DoftTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_AspNetUsers_OwnerId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_Categories_CategoryId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Notes_AspNetUsers_OwnerId",
                table: "Notes");

            migrationBuilder.DropForeignKey(
                name: "FK_Notes_Categories_CategoryId",
                table: "Notes");

            migrationBuilder.DropIndex(
                name: "IX_Notes_DetailId",
                table: "Notes");

            migrationBuilder.DropIndex(
                name: "IX_Events_DetailId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_DoftTasks_DetailId",
                table: "DoftTasks");

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

            migrationBuilder.AlterColumn<string>(
                name: "ItemType",
                table: "TagLinks",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

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

            migrationBuilder.AddColumn<int>(
                name: "EventId",
                table: "Reminders",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NoteId",
                table: "Reminders",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "Events",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "PriorityScore",
                table: "Events",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Events",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "ItemType",
                table: "AttachmentLinks",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

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

            migrationBuilder.CreateIndex(
                name: "IX_Reminders_EventId",
                table: "Reminders",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Reminders_NoteId",
                table: "Reminders",
                column: "NoteId");

            migrationBuilder.CreateIndex(
                name: "IX_Notes_DetailId",
                table: "Notes",
                column: "DetailId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Events_DetailId",
                table: "Events",
                column: "DetailId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DoftTasks_DetailId",
                table: "DoftTasks",
                column: "DetailId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_AspNetUsers_OwnerId",
                table: "Attachments",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DoftTasks_Categories_CategoryId",
                table: "DoftTasks",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_AspNetUsers_OwnerId",
                table: "Events",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Categories_CategoryId",
                table: "Events",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Notes_AspNetUsers_OwnerId",
                table: "Notes",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notes_Categories_CategoryId",
                table: "Notes",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Reminders_Events_EventId",
                table: "Reminders",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reminders_Notes_NoteId",
                table: "Reminders",
                column: "NoteId",
                principalTable: "Notes",
                principalColumn: "Id");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_AspNetUsers_OwnerId",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_DoftTasks_Categories_CategoryId",
                table: "DoftTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_AspNetUsers_OwnerId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_Categories_CategoryId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Notes_AspNetUsers_OwnerId",
                table: "Notes");

            migrationBuilder.DropForeignKey(
                name: "FK_Notes_Categories_CategoryId",
                table: "Notes");

            migrationBuilder.DropForeignKey(
                name: "FK_Reminders_Events_EventId",
                table: "Reminders");

            migrationBuilder.DropForeignKey(
                name: "FK_Reminders_Notes_NoteId",
                table: "Reminders");

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

            migrationBuilder.DropIndex(
                name: "IX_Reminders_EventId",
                table: "Reminders");

            migrationBuilder.DropIndex(
                name: "IX_Reminders_NoteId",
                table: "Reminders");

            migrationBuilder.DropIndex(
                name: "IX_Notes_DetailId",
                table: "Notes");

            migrationBuilder.DropIndex(
                name: "IX_Events_DetailId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_DoftTasks_DetailId",
                table: "DoftTasks");

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

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "Reminders");

            migrationBuilder.DropColumn(
                name: "NoteId",
                table: "Reminders");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "PriorityScore",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Events");

            migrationBuilder.AlterColumn<string>(
                name: "ItemType",
                table: "TagLinks",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "ItemType",
                table: "AttachmentLinks",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.CreateIndex(
                name: "IX_Notes_DetailId",
                table: "Notes",
                column: "DetailId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_DetailId",
                table: "Events",
                column: "DetailId");

            migrationBuilder.CreateIndex(
                name: "IX_DoftTasks_DetailId",
                table: "DoftTasks",
                column: "DetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_AspNetUsers_OwnerId",
                table: "Attachments",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DoftTasks_Categories_CategoryId",
                table: "DoftTasks",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_AspNetUsers_OwnerId",
                table: "Events",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Categories_CategoryId",
                table: "Events",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notes_AspNetUsers_OwnerId",
                table: "Notes",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notes_Categories_CategoryId",
                table: "Notes",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

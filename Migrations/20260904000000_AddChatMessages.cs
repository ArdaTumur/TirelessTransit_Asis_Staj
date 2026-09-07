using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;

#nullable disable

namespace MyAPI.Migrations;

[Migration("20260904000000_AddChatMessages")]
public partial class AddChatMessages : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "ChatMessages",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                ConversationUserId = table.Column<int>(type: "int", nullable: false),
                SenderUserId = table.Column<int>(type: "int", nullable: false),
                Message = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                SentAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ChatMessages", x => x.Id);
                table.ForeignKey("FK_ChatMessages_Users_ConversationUserId", x => x.ConversationUserId, "Users", "Id", onDelete: ReferentialAction.NoAction);
                table.ForeignKey("FK_ChatMessages_Users_SenderUserId", x => x.SenderUserId, "Users", "Id", onDelete: ReferentialAction.NoAction);
            });

        migrationBuilder.CreateIndex(
            name: "IX_ChatMessages_ConversationUserId_SentAt",
            table: "ChatMessages",
            columns: new[] { "ConversationUserId", "SentAt" });

        migrationBuilder.CreateIndex(
            name: "IX_ChatMessages_SenderUserId",
            table: "ChatMessages",
            column: "SenderUserId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "ChatMessages");
    }
}

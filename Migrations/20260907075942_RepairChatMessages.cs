using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyAPI.Migrations
{
    /// <inheritdoc />
    public partial class RepairChatMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF OBJECT_ID(N'[ChatMessages]', N'U') IS NULL
                BEGIN
                    CREATE TABLE [ChatMessages]
                    (
                        [Id] bigint IDENTITY(1,1) NOT NULL,
                        [ConversationUserId] int NOT NULL,
                        [SenderUserId] int NOT NULL,
                        [Message] nvarchar(4000) NOT NULL,
                        [SentAt] datetime2 NOT NULL,
                        CONSTRAINT [PK_ChatMessages] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_ChatMessages_Users_ConversationUserId]
                            FOREIGN KEY ([ConversationUserId]) REFERENCES [Users] ([Id]),
                        CONSTRAINT [FK_ChatMessages_Users_SenderUserId]
                            FOREIGN KEY ([SenderUserId]) REFERENCES [Users] ([Id])
                    );

                    CREATE INDEX [IX_ChatMessages_ConversationUserId_SentAt]
                        ON [ChatMessages] ([ConversationUserId], [SentAt]);
                    CREATE INDEX [IX_ChatMessages_SenderUserId]
                        ON [ChatMessages] ([SenderUserId]);
                END
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddBusLineUserForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_BusLines_CreateUserId",
                table: "BusLines",
                column: "CreateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_BusLines_UpdateUserId",
                table: "BusLines",
                column: "UpdateUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BusLines_Users_CreateUserId",
                table: "BusLines",
                column: "CreateUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_BusLines_Users_UpdateUserId",
                table: "BusLines",
                column: "UpdateUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BusLines_Users_CreateUserId",
                table: "BusLines");

            migrationBuilder.DropForeignKey(
                name: "FK_BusLines_Users_UpdateUserId",
                table: "BusLines");

            migrationBuilder.DropIndex(
                name: "IX_BusLines_CreateUserId",
                table: "BusLines");

            migrationBuilder.DropIndex(
                name: "IX_BusLines_UpdateUserId",
                table: "BusLines");
        }
    }
}

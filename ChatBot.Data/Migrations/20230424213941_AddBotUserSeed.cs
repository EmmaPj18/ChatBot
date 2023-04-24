using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatBot.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBotUserSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "DisplayName", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { 1, 0, "b3bcfca4-4e44-4c99-ae2b-5c050a62ddb2", "Bot", "Bot@chatbot.do", true, false, null, null, null, "AQAAAAIAAYagAAAAEH+G/dsaC/85yFLIOp8fOqd9lrpCEke10JHOIaIG5gHrHGn6eyrQR+AeLf+mCs6FyA==", null, false, null, false, "Bot@chatbot.do" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}

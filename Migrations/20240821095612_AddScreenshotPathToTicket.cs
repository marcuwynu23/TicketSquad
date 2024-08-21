using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketSquad.Migrations
{
    /// <inheritdoc />
    public partial class AddScreenshotPathToTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ScreenshotPath",
                table: "Tickets",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ScreenshotPath",
                table: "Tickets");
        }
    }
}

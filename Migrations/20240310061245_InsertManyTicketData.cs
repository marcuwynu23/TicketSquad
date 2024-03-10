using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketSquad.Migrations
{
    /// <inheritdoc />
    public partial class InsertManyTicketData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Tickets",
                columns: new[]
                {
                    "Title",
                    "Description",
                    "Status",
                    "Priority",
                    "CreatedAt",
                    "UpdatedAt",
                    "UserId"
                },
                values: new object[]
                {
                    "Coordinate Team Building Workshop",
                    "Organize a workshop to enhance team communication and collaboration skills.",
                    0,
                    2,
                    DateTime.UtcNow,
                    DateTime.UtcNow,
                    1
                }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM public.Tickets");
        }
    }
}

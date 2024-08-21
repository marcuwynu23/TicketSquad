using Microsoft.EntityFrameworkCore.Migrations;
using TicketSquad.Models;
#nullable disable

namespace TicketSquad.Migrations
{
    /// <inheritdoc />
    public partial class InsertManyTicketData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            Random random = new Random();

            for (int i = 0; i < 20; i++)
            {
                string[] titles =
                {
                    "Coordinate Team Building Workshop",
                    "Fix Bug in Backend System",
                    "Upgrade Server Hardware",
                    "Design New Logo",
                    "Implement Payment Gateway",
                    "Write Documentation for API",
                    "Optimize Database Queries",
                    "Plan Marketing Campaign",
                    "Create User Interface Mockups",
                    "Deploy Application to Production",
                    "Train Customer Support Team",
                    "Research New Technologies",
                    "Review and Approve Expenses",
                    "Solve Network Connectivity Issues",
                    "Develop Mobile App Feature",
                    "Conduct Security Audit",
                    "Resolve Customer Complaints",
                    "Improve Website Performance",
                    "Debug Integration Errors",
                    "Organize Team Building Retreat"
                };

                string title = titles[random.Next(titles.Length)];
                string description =
                    $"This is a description for ticket {i + 1}. Lorem ipsum dolor sit amet, consectetur adipiscing elit.";
                TicketStatus status = (TicketStatus)
                    random.Next(Enum.GetValues(typeof(TicketStatus)).Length);
                TicketPriority priority = (TicketPriority)
                    random.Next(Enum.GetValues(typeof(TicketPriority)).Length);
                DateTime createdAt = DateTime.UtcNow.AddDays(-random.Next(1, 30)); // Random date within the last 30 days
                DateTime updatedAt = createdAt.AddDays(random.Next(1, 15)); // Random date between createdAt and 15 days ahead

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
                        title,
                        description,
                        (int)status,
                        (int)priority,
                        createdAt,
                        updatedAt,
                        2 // Fixed user ID to 1
                    }
                );
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("Tickets");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;
#nullable disable

namespace TicketSquad.Migrations
{
    /// <inheritdoc />
    public partial class InsertAdminData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //insert admin data
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[]
                {
                    "Id",
                    "FirstName",
                    "MiddleName",
                    "LastName",
                    "Email",
                    "Password",
                    "Role",
                    "CreatedAt",
                    "UpdatedAt"
                },
                values: new object[]
                {
                    1,
                    "Mark Wayne",
                    "Buncaras",
                    "Menorca",
                    "admin@admin.com",
                    "admin",
                    0,
                    DateTime.UtcNow,
                    DateTime.UtcNow
                }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(table: "Users", keyColumn: "Id", keyValue: 1);
        }
    }
}

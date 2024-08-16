using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication2.Migrations
{
    /// <inheritdoc />
    public partial class Initialize3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SmsResponses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SmsNotificationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SmsNotificationActionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TwilioPhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserPhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmsResponses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SmsResponses_SmsNotificationActions_SmsNotificationActionId",
                        column: x => x.SmsNotificationActionId,
                        principalTable: "SmsNotificationActions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SmsResponses_SmsNotifications_SmsNotificationId",
                        column: x => x.SmsNotificationId,
                        principalTable: "SmsNotifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SmsResponses_SmsNotificationActionId",
                table: "SmsResponses",
                column: "SmsNotificationActionId");

            migrationBuilder.CreateIndex(
                name: "IX_SmsResponses_SmsNotificationId",
                table: "SmsResponses",
                column: "SmsNotificationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SmsResponses");
        }
    }
}

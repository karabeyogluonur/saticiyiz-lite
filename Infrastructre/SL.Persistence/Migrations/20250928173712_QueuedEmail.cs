using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SL.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class QueuedEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QueuedEmails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    From = table.Column<string>(type: "text", nullable: false),
                    To = table.Column<string>(type: "text", nullable: false),
                    Cc = table.Column<string>(type: "text", nullable: true),
                    Bcc = table.Column<string>(type: "text", nullable: true),
                    Subject = table.Column<string>(type: "text", nullable: false),
                    Body = table.Column<string>(type: "text", nullable: false),
                    EmailAccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    SentTries = table.Column<int>(type: "integer", nullable: false),
                    SentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    NextTryAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastError = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QueuedEmails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QueuedEmails_EmailAccounts_EmailAccountId",
                        column: x => x.EmailAccountId,
                        principalTable: "EmailAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QueuedEmails_EmailAccountId",
                table: "QueuedEmails",
                column: "EmailAccountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QueuedEmails");
        }
    }
}

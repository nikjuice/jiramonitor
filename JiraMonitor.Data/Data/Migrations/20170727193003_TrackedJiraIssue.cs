using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace JiraMonitor.Data.Migrations
{
    public partial class TrackedJiraIssue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TrackedJiraIssue",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    JiraIssueKey = table.Column<string>(maxLength: 20, nullable: true),
                    JqlQueryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackedJiraIssue", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TrackedJiraIssue_JqlQuery_JqlQueryId",
                        column: x => x.JqlQueryId,
                        principalTable: "JqlQuery",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrackedJiraIssue_JqlQueryId",
                table: "TrackedJiraIssue",
                column: "JqlQueryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrackedJiraIssue");
        }
    }
}

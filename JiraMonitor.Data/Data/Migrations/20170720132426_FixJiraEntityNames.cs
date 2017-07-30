using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace JiraMonitor.Data.Migrations
{
    public partial class FixJiraEntityNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "JiraUser",
                table: "JiraSettings",
                newName: "User");

            migrationBuilder.RenameColumn(
                name: "JiraUrl",
                table: "JiraSettings",
                newName: "Url");

            migrationBuilder.RenameColumn(
                name: "JiraPassword",
                table: "JiraSettings",
                newName: "Password");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "User",
                table: "JiraSettings",
                newName: "JiraUser");

            migrationBuilder.RenameColumn(
                name: "Url",
                table: "JiraSettings",
                newName: "JiraUrl");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "JiraSettings",
                newName: "JiraPassword");
        }
    }
}

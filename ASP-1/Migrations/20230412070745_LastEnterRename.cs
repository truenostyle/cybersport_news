using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASP_1.Migrations
{
    /// <inheritdoc />
    public partial class LastEnterRename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastEnter",
                table: "Users");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastEnterDt",
                table: "Users",
                type: "datetime(6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastEnterDt",
                table: "Users");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastEnter",
                table: "Users",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}

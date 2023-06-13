using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASP_1.Migrations
{
    /// <inheritdoc />
    public partial class AddPublicFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDtPublic",
                table: "Users",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRealNamePublic",
                table: "Users",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsShareEmail",
                table: "Users",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDtPublic",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsRealNamePublic",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsShareEmail",
                table: "Users");
        }
    }
}

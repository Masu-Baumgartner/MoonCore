using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoonCore.Blazor.Test.Database.Migrations
{
    /// <inheritdoc />
    public partial class Something3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DriverType",
                table: "Cars",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DriverType",
                table: "Cars");
        }
    }
}

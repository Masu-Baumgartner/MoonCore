using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoonCore.Blazor.Test.Database.Migrations
{
    /// <inheritdoc />
    public partial class Something2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsElectric",
                table: "Cars",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsElectric",
                table: "Cars");
        }
    }
}

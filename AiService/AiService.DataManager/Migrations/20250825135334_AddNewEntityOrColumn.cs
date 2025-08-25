using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AiService.DataManager.Migrations
{
    /// <inheritdoc />
    public partial class AddNewEntityOrColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Categories",
                newName: "Email");

            migrationBuilder.AddColumn<string>(
                name: "ConfirmPassword",
                table: "Categories",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConfirmPassword",
                table: "Categories");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Categories",
                newName: "Name");
        }
    }
}

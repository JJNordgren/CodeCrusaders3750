using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CS3750Assignment1.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceNameWithFirstAndLastName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "name",
                table: "Users",
                newName: "lastName");

            migrationBuilder.AddColumn<string>(
                name: "firstName",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "firstName",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "lastName",
                table: "Users",
                newName: "name");
        }
    }
}

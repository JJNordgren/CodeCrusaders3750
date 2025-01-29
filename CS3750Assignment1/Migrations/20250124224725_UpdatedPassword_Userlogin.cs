using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CS3750Assignment1.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedPassword_Userlogin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "confirmPassword",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "confirmPassword",
                table: "Users");
        }
    }
}

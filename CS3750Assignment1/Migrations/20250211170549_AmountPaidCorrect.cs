using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CS3750Assignment1.Migrations
{
    /// <inheritdoc />
    public partial class AmountPaidCorrect : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Check if the column already exists before adding it
            migrationBuilder.Sql(@"
                IF COL_LENGTH('Users', 'addressLine1') IS NULL
                BEGIN
                    ALTER TABLE Users ADD addressLine1 NVARCHAR(MAX) NULL
                END
            ");

            migrationBuilder.Sql(@"
                IF COL_LENGTH('Users', 'addressLine2') IS NULL
                BEGIN
                    ALTER TABLE Users ADD addressLine2 NVARCHAR(MAX) NULL
                END
            ");

            migrationBuilder.Sql(@"
                IF COL_LENGTH('Users', 'amountPaid') IS NULL
                BEGIN
                    ALTER TABLE Users ADD amountPaid FLOAT NOT NULL DEFAULT 0.0
                END
            ");

            migrationBuilder.Sql(@"
                IF COL_LENGTH('Users', 'city') IS NULL
                BEGIN
                    ALTER TABLE Users ADD city NVARCHAR(MAX) NULL
                END
            ");

            migrationBuilder.Sql(@"
                IF COL_LENGTH('Users', 'name') IS NULL
                BEGIN
                    ALTER TABLE Users ADD name NVARCHAR(MAX) NOT NULL DEFAULT ''
                END
            ");

            migrationBuilder.Sql(@"
                IF COL_LENGTH('Users', 'phone') IS NULL
                BEGIN
                    ALTER TABLE Users ADD phone NVARCHAR(MAX) NULL
                END
            ");

            migrationBuilder.Sql(@"
                IF COL_LENGTH('Users', 'state') IS NULL
                BEGIN
                    ALTER TABLE Users ADD state NVARCHAR(MAX) NULL
                END
            ");

            migrationBuilder.Sql(@"
                IF COL_LENGTH('Users', 'zip') IS NULL
                BEGIN
                    ALTER TABLE Users ADD zip NVARCHAR(MAX) NULL
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "addressLine1",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "addressLine2",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "amountPaid",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "city",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "name",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "phone",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "state",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "zip",
                table: "Users");
        }
    }
}



//using Microsoft.EntityFrameworkCore.Migrations;

//#nullable disable

//namespace CS3750Assignment1.Migrations
//{
//    /// <inheritdoc />
//    public partial class AmountPaidCorrect : Migration
//    {
//        /// <inheritdoc />
//        protected override void Up(MigrationBuilder migrationBuilder)
//        {
//            migrationBuilder.AddColumn<string>(
//                name: "addressLine1",
//                table: "Users",
//                type: "nvarchar(max)",
//                nullable: true);

//            migrationBuilder.AddColumn<string>(
//                name: "addressLine2",
//                table: "Users",
//                type: "nvarchar(max)",
//                nullable: true);

//            migrationBuilder.AddColumn<double>(
//                name: "amountPaid",
//                table: "Users",
//                type: "float",
//                nullable: true);

//            migrationBuilder.AddColumn<string>(
//                name: "city",
//                table: "Users",
//                type: "nvarchar(max)",
//                nullable: true);

//            migrationBuilder.AddColumn<string>(
//                name: "name",
//                table: "Users",
//                type: "nvarchar(max)",
//                nullable: false,
//                defaultValue: "");

//            migrationBuilder.AddColumn<string>(
//                name: "phone",
//                table: "Users",
//                type: "nvarchar(max)",
//                nullable: true);

//            migrationBuilder.AddColumn<string>(
//                name: "state",
//                table: "Users",
//                type: "nvarchar(max)",
//                nullable: true);

//            migrationBuilder.AddColumn<string>(
//                name: "zip",
//                table: "Users",
//                type: "nvarchar(max)",
//                nullable: true);
//        }

//        /// <inheritdoc />
//        protected override void Down(MigrationBuilder migrationBuilder)
//        {
//            migrationBuilder.DropColumn(
//                name: "addressLine1",
//                table: "Users");

//            migrationBuilder.DropColumn(
//                name: "addressLine2",
//                table: "Users");

//            migrationBuilder.DropColumn(
//                name: "amountPaid",
//                table: "Users");

//            migrationBuilder.DropColumn(
//                name: "city",
//                table: "Users");

//            migrationBuilder.DropColumn(
//                name: "name",
//                table: "Users");

//            migrationBuilder.DropColumn(
//                name: "phone",
//                table: "Users");

//            migrationBuilder.DropColumn(
//                name: "state",
//                table: "Users");

//            migrationBuilder.DropColumn(
//                name: "zip",
//                table: "Users");
//        }
//    }
//}

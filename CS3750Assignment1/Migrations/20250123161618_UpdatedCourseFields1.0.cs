using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CS3750Assignment1.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedCourseFields10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeSpan>(
                name: "EndTime",
                table: "Courses",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<string>(
                name: "MeetingDays",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "StartTime",
                table: "Courses",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "MeetingDays",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "Courses");
        }
    }
}

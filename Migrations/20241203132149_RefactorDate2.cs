using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tsisa.Blockchain.Migrations
{
    /// <inheritdoc />
    public partial class RefactorDate2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArbiterTimestamp",
                table: "Blocks");

            migrationBuilder.AddColumn<string>(
                name: "ArbiterTimestampString",
                table: "Blocks",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArbiterTimestampString",
                table: "Blocks");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ArbiterTimestamp",
                table: "Blocks",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}

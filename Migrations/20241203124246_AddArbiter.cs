using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tsisa.Blockchain.Migrations
{
    /// <inheritdoc />
    public partial class AddArbiter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ArbiterSignature",
                table: "Blocks",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ArbiterTimestamp",
                table: "Blocks",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArbiterSignature",
                table: "Blocks");

            migrationBuilder.DropColumn(
                name: "ArbiterTimestamp",
                table: "Blocks");
        }
    }
}

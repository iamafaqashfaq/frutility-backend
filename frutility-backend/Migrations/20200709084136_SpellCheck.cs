using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace frutility_backend.Migrations
{
    public partial class SpellCheck : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdataionDate",
                table: "Products");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdationDate",
                table: "Products",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdationDate",
                table: "Products");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdataionDate",
                table: "Products",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace frutility_backend.Migrations
{
    public partial class CustomiseIdentity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BillingAddress",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillingCity",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BillingPincode",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "BillingState",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RegDate",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ShippingAddress",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShippingCity",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShippingPincode",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ShippingState",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdationDate",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BillingAddress",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "BillingCity",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "BillingPincode",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "BillingState",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RegDate",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ShippingAddress",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ShippingCity",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ShippingPincode",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ShippingState",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UpdationDate",
                table: "AspNetUsers");
        }
    }
}

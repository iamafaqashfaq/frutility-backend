using Microsoft.EntityFrameworkCore.Migrations;

namespace frutility_backend.Migrations
{
    public partial class AddedOrderRemarks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "Orders",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "Orders");
        }
    }
}

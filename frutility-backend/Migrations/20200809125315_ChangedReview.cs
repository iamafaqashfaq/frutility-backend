using Microsoft.EntityFrameworkCore.Migrations;

namespace frutility_backend.Migrations
{
    public partial class ChangedReview : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Summary",
                table: "ProductReviews");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Summary",
                table: "ProductReviews",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}

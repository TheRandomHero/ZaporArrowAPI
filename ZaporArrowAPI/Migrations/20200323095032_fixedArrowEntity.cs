using Microsoft.EntityFrameworkCore.Migrations;

namespace ZaporArrowAPI.Migrations
{
    public partial class fixedArrowEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Length",
                table: "Arrows");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Length",
                table: "Arrows",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}

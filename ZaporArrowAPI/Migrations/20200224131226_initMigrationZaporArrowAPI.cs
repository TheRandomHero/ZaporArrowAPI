using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ZaporArrowAPI.Migrations
{
    public partial class initMigrationZaporArrowAPI : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Arrows",
                columns: table => new
                {
                    ArrowId = table.Column<Guid>(nullable: false),
                    Length = table.Column<double>(nullable: false),
                    Description = table.Column<string>(maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Arrows", x => x.ArrowId);
                });

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    ImageId = table.Column<Guid>(nullable: false),
                    ArrowId = table.Column<Guid>(nullable: false),
                    ImageSource = table.Column<string>(nullable: false),
                    isProfilePicture = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.ImageId);
                    table.ForeignKey(
                        name: "FK_Images_Arrows_ArrowId",
                        column: x => x.ArrowId,
                        principalTable: "Arrows",
                        principalColumn: "ArrowId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Images_ArrowId",
                table: "Images",
                column: "ArrowId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropTable(
                name: "Arrows");
        }
    }
}

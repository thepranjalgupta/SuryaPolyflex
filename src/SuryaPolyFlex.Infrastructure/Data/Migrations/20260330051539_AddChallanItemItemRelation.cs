using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SuryaPolyFlex.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddChallanItemItemRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChallanItems_Items_ItemId",
                schema: "dispatch",
                table: "ChallanItems");

            migrationBuilder.AddForeignKey(
                name: "FK_ChallanItems_Items_ItemId",
                schema: "dispatch",
                table: "ChallanItems",
                column: "ItemId",
                principalSchema: "inv",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChallanItems_Items_ItemId",
                schema: "dispatch",
                table: "ChallanItems");

            migrationBuilder.AddForeignKey(
                name: "FK_ChallanItems_Items_ItemId",
                schema: "dispatch",
                table: "ChallanItems",
                column: "ItemId",
                principalSchema: "inv",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

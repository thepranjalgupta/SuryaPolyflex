using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SuryaPolyFlex.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class REAddProcurementTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GRNs_PurchaseOrders_PurchaseOrderId",
                schema: "proc",
                table: "GRNs");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrders_Vendors_VendorId",
                schema: "proc",
                table: "PurchaseOrders");

            migrationBuilder.AddForeignKey(
                name: "FK_GRNs_PurchaseOrders_PurchaseOrderId",
                schema: "proc",
                table: "GRNs",
                column: "PurchaseOrderId",
                principalSchema: "proc",
                principalTable: "PurchaseOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrders_Vendors_VendorId",
                schema: "proc",
                table: "PurchaseOrders",
                column: "VendorId",
                principalSchema: "proc",
                principalTable: "Vendors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GRNs_PurchaseOrders_PurchaseOrderId",
                schema: "proc",
                table: "GRNs");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrders_Vendors_VendorId",
                schema: "proc",
                table: "PurchaseOrders");

            migrationBuilder.AddForeignKey(
                name: "FK_GRNs_PurchaseOrders_PurchaseOrderId",
                schema: "proc",
                table: "GRNs",
                column: "PurchaseOrderId",
                principalSchema: "proc",
                principalTable: "PurchaseOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrders_Vendors_VendorId",
                schema: "proc",
                table: "PurchaseOrders",
                column: "VendorId",
                principalSchema: "proc",
                principalTable: "Vendors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SuryaPolyFlex.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddProcurement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Indents",
                schema: "proc",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IndentNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IndentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    RequestedById = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovedById = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Indents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Indents_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalSchema: "dbo",
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IndentItems",
                schema: "proc",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IndentId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    RequestedQty = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    ApprovedQty = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndentItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IndentItems_Indents_IndentId",
                        column: x => x.IndentId,
                        principalSchema: "proc",
                        principalTable: "Indents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IndentItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalSchema: "inv",
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrders",
                schema: "proc",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PONumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PODate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VendorId = table.Column<int>(type: "int", nullable: false),
                    IndentId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ExpectedDeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Terms = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ApprovedById = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseOrders_Indents_IndentId",
                        column: x => x.IndentId,
                        principalSchema: "proc",
                        principalTable: "Indents",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PurchaseOrders_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalSchema: "proc",
                        principalTable: "Vendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GRNs",
                schema: "proc",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GRNNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    GRNDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PurchaseOrderId = table.Column<int>(type: "int", nullable: false),
                    VendorId = table.Column<int>(type: "int", nullable: false),
                    VehicleNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeliveryNoteNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    QCRemarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GRNs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GRNs_PurchaseOrders_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalSchema: "proc",
                        principalTable: "PurchaseOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GRNs_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalSchema: "inv",
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "POItems",
                schema: "proc",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseOrderId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    OrderedQty = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    ReceivedQty = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxPct = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_POItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_POItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalSchema: "inv",
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_POItems_PurchaseOrders_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalSchema: "proc",
                        principalTable: "PurchaseOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GRNItems",
                schema: "proc",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GRNId = table.Column<int>(type: "int", nullable: false),
                    POItemId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    ReceivedQty = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    AcceptedQty = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    RejectedQty = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    RejectionReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UnitCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GRNItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GRNItems_GRNs_GRNId",
                        column: x => x.GRNId,
                        principalSchema: "proc",
                        principalTable: "GRNs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GRNItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalSchema: "inv",
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GRNItems_GRNId",
                schema: "proc",
                table: "GRNItems",
                column: "GRNId");

            migrationBuilder.CreateIndex(
                name: "IX_GRNItems_ItemId",
                schema: "proc",
                table: "GRNItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_GRNs_GRNNumber",
                schema: "proc",
                table: "GRNs",
                column: "GRNNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GRNs_PurchaseOrderId",
                schema: "proc",
                table: "GRNs",
                column: "PurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_GRNs_WarehouseId",
                schema: "proc",
                table: "GRNs",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_IndentItems_IndentId",
                schema: "proc",
                table: "IndentItems",
                column: "IndentId");

            migrationBuilder.CreateIndex(
                name: "IX_IndentItems_ItemId",
                schema: "proc",
                table: "IndentItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Indents_DepartmentId",
                schema: "proc",
                table: "Indents",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Indents_IndentNumber",
                schema: "proc",
                table: "Indents",
                column: "IndentNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_POItems_ItemId",
                schema: "proc",
                table: "POItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_POItems_PurchaseOrderId",
                schema: "proc",
                table: "POItems",
                column: "PurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_IndentId",
                schema: "proc",
                table: "PurchaseOrders",
                column: "IndentId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_PONumber",
                schema: "proc",
                table: "PurchaseOrders",
                column: "PONumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_VendorId",
                schema: "proc",
                table: "PurchaseOrders",
                column: "VendorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GRNItems",
                schema: "proc");

            migrationBuilder.DropTable(
                name: "IndentItems",
                schema: "proc");

            migrationBuilder.DropTable(
                name: "POItems",
                schema: "proc");

            migrationBuilder.DropTable(
                name: "GRNs",
                schema: "proc");

            migrationBuilder.DropTable(
                name: "PurchaseOrders",
                schema: "proc");

            migrationBuilder.DropTable(
                name: "Indents",
                schema: "proc");
        }
    }
}

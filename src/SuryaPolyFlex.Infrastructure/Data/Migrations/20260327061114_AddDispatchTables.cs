using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SuryaPolyFlex.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDispatchTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dispatch");

            migrationBuilder.CreateTable(
                name: "DispatchPlans",
                schema: "dispatch",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlanNo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PlannedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DispatchPlans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Transporters",
                schema: "dispatch",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactPerson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GSTIN = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transporters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DispatchPlanItems",
                schema: "dispatch",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DispatchPlanId = table.Column<int>(type: "int", nullable: false),
                    SOId = table.Column<int>(type: "int", nullable: false),
                    DispatchQty = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DispatchPlanItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DispatchPlanItems_DispatchPlans_DispatchPlanId",
                        column: x => x.DispatchPlanId,
                        principalSchema: "dispatch",
                        principalTable: "DispatchPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DispatchPlanItems_SalesOrders_SOId",
                        column: x => x.SOId,
                        principalSchema: "sales",
                        principalTable: "SalesOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DeliveryChallans",
                schema: "dispatch",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChallanNo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DispatchPlanId = table.Column<int>(type: "int", nullable: true),
                    SOId = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    ChallanDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TransporterId = table.Column<int>(type: "int", nullable: true),
                    VehicleNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DriverName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LRNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EWayBillNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeliveryAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalValue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryChallans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeliveryChallans_DispatchPlans_DispatchPlanId",
                        column: x => x.DispatchPlanId,
                        principalSchema: "dispatch",
                        principalTable: "DispatchPlans",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DeliveryChallans_SalesOrders_SOId",
                        column: x => x.SOId,
                        principalSchema: "sales",
                        principalTable: "SalesOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DeliveryChallans_Transporters_TransporterId",
                        column: x => x.TransporterId,
                        principalSchema: "dispatch",
                        principalTable: "Transporters",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ChallanItems",
                schema: "dispatch",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChallanId = table.Column<int>(type: "int", nullable: false),
                    SOItemId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    DispatchedQty = table.Column<decimal>(type: "decimal(18,3)", precision: 18, scale: 3, nullable: false),
                    UoMId = table.Column<int>(type: "int", nullable: true),
                    UnitRate = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChallanItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChallanItems_DeliveryChallans_ChallanId",
                        column: x => x.ChallanId,
                        principalSchema: "dispatch",
                        principalTable: "DeliveryChallans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChallanItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalSchema: "inv",
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChallanItems_SOItems_SOItemId",
                        column: x => x.SOItemId,
                        principalSchema: "sales",
                        principalTable: "SOItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Shipments",
                schema: "dispatch",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChallanId = table.Column<int>(type: "int", nullable: false),
                    CurrentStatus = table.Column<int>(type: "int", nullable: false),
                    StatusUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StatusUpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EstimatedDeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualDeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PODDocument = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiverName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeliveryRemarks = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shipments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Shipments_DeliveryChallans_ChallanId",
                        column: x => x.ChallanId,
                        principalSchema: "dispatch",
                        principalTable: "DeliveryChallans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChallanItems_ChallanId",
                schema: "dispatch",
                table: "ChallanItems",
                column: "ChallanId");

            migrationBuilder.CreateIndex(
                name: "IX_ChallanItems_ItemId",
                schema: "dispatch",
                table: "ChallanItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ChallanItems_SOItemId",
                schema: "dispatch",
                table: "ChallanItems",
                column: "SOItemId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryChallans_ChallanNo",
                schema: "dispatch",
                table: "DeliveryChallans",
                column: "ChallanNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryChallans_DispatchPlanId",
                schema: "dispatch",
                table: "DeliveryChallans",
                column: "DispatchPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryChallans_SOId",
                schema: "dispatch",
                table: "DeliveryChallans",
                column: "SOId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryChallans_TransporterId",
                schema: "dispatch",
                table: "DeliveryChallans",
                column: "TransporterId");

            migrationBuilder.CreateIndex(
                name: "IX_DispatchPlanItems_DispatchPlanId",
                schema: "dispatch",
                table: "DispatchPlanItems",
                column: "DispatchPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_DispatchPlanItems_SOId",
                schema: "dispatch",
                table: "DispatchPlanItems",
                column: "SOId");

            migrationBuilder.CreateIndex(
                name: "IX_DispatchPlans_PlanNo",
                schema: "dispatch",
                table: "DispatchPlans",
                column: "PlanNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_ChallanId",
                schema: "dispatch",
                table: "Shipments",
                column: "ChallanId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChallanItems",
                schema: "dispatch");

            migrationBuilder.DropTable(
                name: "DispatchPlanItems",
                schema: "dispatch");

            migrationBuilder.DropTable(
                name: "Shipments",
                schema: "dispatch");

            migrationBuilder.DropTable(
                name: "DeliveryChallans",
                schema: "dispatch");

            migrationBuilder.DropTable(
                name: "DispatchPlans",
                schema: "dispatch");

            migrationBuilder.DropTable(
                name: "Transporters",
                schema: "dispatch");
        }
    }
}

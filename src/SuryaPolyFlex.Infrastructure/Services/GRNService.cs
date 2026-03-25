using Microsoft.EntityFrameworkCore;
using SuryaPolyFlex.Application.Common.Interfaces;
using SuryaPolyFlex.Application.Features.GRN;
using SuryaPolyFlex.Domain.Entities.Procurement;
using SuryaPolyFlex.Domain.Enums;
using SuryaPolyFlex.Infrastructure.Data;

namespace SuryaPolyFlex.Infrastructure.Services;

public class GRNService : IGRNService
{
    private readonly AppDbContext _context;
    private readonly INumberSequenceService _numberService;
    private readonly IStockService _stockService;

    public GRNService(
        AppDbContext context,
        INumberSequenceService numberService,
        IStockService stockService)
    {
        _context       = context;
        _numberService = numberService;
        _stockService  = stockService;
    }

    public async Task<List<GRNDto>> GetAllAsync()
    {
        return await _context.GRNs
            .Include(g => g.PurchaseOrder)
                .ThenInclude(p => p.Vendor)
            .OrderByDescending(g => g.CreatedAt)
            .Select(g => new GRNDto
            {
                Id                 = g.Id,
                GRNNumber          = g.GRNNumber,
                GRNDate            = g.GRNDate,
                PurchaseOrderId    = g.PurchaseOrderId,
                PONumber           = g.PurchaseOrder.PONumber,
                VendorName         = g.PurchaseOrder.Vendor.Name,
                VehicleNumber      = g.VehicleNumber,
                DeliveryNoteNumber = g.DeliveryNoteNumber,
                Status             = g.Status.ToString(),
                Remarks            = g.Remarks
            })
            .ToListAsync();
    }

    public async Task<GRNDto?> GetByIdAsync(int id)
    {
        var grn = await _context.GRNs
            .Include(g => g.PurchaseOrder)
                .ThenInclude(p => p.Vendor)
            .Include(g => g.Items)
            .Include(g => g.Warehouse)
            .FirstOrDefaultAsync(g => g.Id == id);

        if (grn == null) return null;

        var itemIds = grn.Items.Select(i => i.ItemId).ToList();
        var items = await _context.Items
            .Include(i => i.UoM)
            .Where(i => itemIds.Contains(i.Id))
            .ToDictionaryAsync(i => i.Id);

        return new GRNDto
        {
            Id                 = grn.Id,
            GRNNumber          = grn.GRNNumber,
            GRNDate            = grn.GRNDate,
            PurchaseOrderId    = grn.PurchaseOrderId,
            PONumber           = grn.PurchaseOrder.PONumber,
            VendorName         = grn.PurchaseOrder.Vendor.Name,
            VehicleNumber      = grn.VehicleNumber,
            DeliveryNoteNumber = grn.DeliveryNoteNumber,
            Status             = grn.Status.ToString(),
            WarehouseName      = grn.Warehouse?.Name ?? "",
            Remarks            = grn.Remarks,
            Items              = grn.Items.Select(i => new GRNItemDto
            {
                Id              = i.Id,
                ItemId          = i.ItemId,
                ItemCode        = items.ContainsKey(i.ItemId) ? items[i.ItemId].ItemCode : "",
                ItemName        = items.ContainsKey(i.ItemId) ? items[i.ItemId].Name : "",
                UoMCode         = items.ContainsKey(i.ItemId) ? items[i.ItemId].UoM.Code : "",
                ReceivedQty     = i.ReceivedQty,
                AcceptedQty     = i.AcceptedQty,
                RejectedQty     = i.RejectedQty,
                RejectionReason = i.RejectionReason,
                UnitCost        = i.UnitCost
            }).ToList()
        };
    }

    public async Task<List<GRNItemDto>> GetItemsForPOAsync(int poId)
    {
        var poItems = await _context.POItems
            .Include(p => p.PurchaseOrder)
            .Where(p => p.PurchaseOrderId == poId)
            .ToListAsync();

        var itemIds = poItems.Select(p => p.ItemId).ToList();
        var items = await _context.Items
            .Include(i => i.UoM)
            .Where(i => itemIds.Contains(i.Id))
            .ToDictionaryAsync(i => i.Id);

        return poItems.Select(p => new GRNItemDto
        {
            Id         = p.Id,
            ItemId     = p.ItemId,
            ItemCode   = items.ContainsKey(p.ItemId) ? items[p.ItemId].ItemCode : "",
            ItemName   = items.ContainsKey(p.ItemId) ? items[p.ItemId].Name : "",
            UoMCode    = items.ContainsKey(p.ItemId) ? items[p.ItemId].UoM.Code : "",
            ReceivedQty = p.OrderedQty - p.ReceivedQty, // pending qty as default
            UnitCost   = p.UnitPrice
        }).ToList();
    }

    public async Task<(bool Success, string Message, int GRNId)> CreateAsync(
        CreateGRNDto dto, string userName)
    {
        var po = await _context.PurchaseOrders
            .Include(p => p.Items)
            .FirstOrDefaultAsync(p => p.Id == dto.PurchaseOrderId);

        if (po == null)
            return (false, "Purchase Order not found.", 0);

        if (po.Status == PurchaseOrderStatus.Closed ||
            po.Status == PurchaseOrderStatus.Cancelled)
            return (false, "Cannot create GRN for closed or cancelled PO.", 0);

        var grnNumber = await _numberService.GenerateAsync("GRN");

        var grn = new GRN
        {
            GRNNumber          = grnNumber,
            GRNDate            = DateTime.UtcNow,
            PurchaseOrderId    = dto.PurchaseOrderId,
            VendorId           = po.VendorId,
            WarehouseId        = dto.WarehouseId,
            VehicleNumber      = dto.VehicleNumber,
            DeliveryNoteNumber = dto.DeliveryNoteNumber,
            Status             = GrnStatus.Pending,
            Remarks            = dto.Remarks,
            CreatedBy          = userName,
            CreatedAt          = DateTime.UtcNow,
            Items = dto.Items.Select(i => new GRNItem
            {
                POItemId        = i.POItemId,
                ItemId          = i.ItemId,
                ReceivedQty     = i.ReceivedQty,
                AcceptedQty     = i.AcceptedQty,
                RejectedQty     = i.ReceivedQty - i.AcceptedQty,
                RejectionReason = i.RejectionReason,
                UnitCost        = i.UnitCost,
                CreatedBy       = userName,
                CreatedAt       = DateTime.UtcNow
            }).ToList()
        };

        _context.GRNs.Add(grn);
        await _context.SaveChangesAsync();

        return (true, $"GRN {grnNumber} created successfully.", grn.Id);
    }

    public async Task<(bool Success, string Message)> AcceptAsync(
        int id, string userName)
    {
        var grn = await _context.GRNs
            .Include(g => g.Items)
            .Include(g => g.PurchaseOrder)
                .ThenInclude(p => p.Items)
            .FirstOrDefaultAsync(g => g.Id == id);

        if (grn == null) return (false, "GRN not found.");
        if (grn.Status != GrnStatus.Pending)
            return (false, "GRN is already processed.");

        // Update stock for each accepted item
        foreach (var item in grn.Items)
        {
            if (item.AcceptedQty > 0)
            {
                await _stockService.ReceiveStockAsync(
                    itemId:          item.ItemId,
                    warehouseId:     grn.WarehouseId,
                    qty:             item.AcceptedQty,
                    unitCost:        item.UnitCost,
                    transactionType: StockTransactionType.GRNReceipt,
                    referenceType:   "GRN",
                    referenceId:     grn.Id,
                    referenceNumber: grn.GRNNumber,
                    createdBy:       userName
                );
            }

            // Update PO received qty
            var poItem = grn.PurchaseOrder.Items
                .FirstOrDefault(p => p.Id == item.POItemId);
            if (poItem != null)
                poItem.ReceivedQty += item.AcceptedQty;
        }

        // Update GRN status
        grn.Status    = GrnStatus.Accepted;
        grn.UpdatedAt = DateTime.UtcNow;
        grn.UpdatedBy = userName;

        // Check if PO is fully received
        var po = grn.PurchaseOrder;
        var allReceived = po.Items.All(i => i.ReceivedQty >= i.OrderedQty);
        po.Status = allReceived
            ? PurchaseOrderStatus.Closed
            : PurchaseOrderStatus.PartialReceived;

        await _context.SaveChangesAsync();
        return (true, "GRN accepted and stock updated successfully.");
    }
}
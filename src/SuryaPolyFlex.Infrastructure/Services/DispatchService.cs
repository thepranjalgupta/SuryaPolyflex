using Microsoft.EntityFrameworkCore;
using SuryaPolyFlex.Application.Common.Interfaces;
using SuryaPolyFlex.Application.Features.Dispatch;
using SuryaPolyFlex.Domain.Entities.Dispatch;
using SuryaPolyFlex.Domain.Enums;
using SuryaPolyFlex.Infrastructure.Data;

namespace SuryaPolyFlex.Infrastructure.Services;

public class DispatchService : IDispatchService
{
    private readonly AppDbContext _context;
    private readonly INumberSequenceService _numberService;
    private readonly IStockService _stockService;

    public DispatchService(
        AppDbContext context,
        INumberSequenceService numberService,
        IStockService stockService)
    {
        _context       = context;
        _numberService = numberService;
        _stockService  = stockService;
    }

    // ── TRANSPORTERS ──────────────────────────────────────────────────────
    public async Task<List<TransporterDto>> GetTransportersAsync()
    {
        return await _context.Transporters
            .Where(t => t.IsActive)
            .OrderBy(t => t.Name)
            .Select(t => new TransporterDto
            {
                Id            = t.Id,
                Name          = t.Name,
                ContactPerson = t.ContactPerson,
                Phone         = t.Phone,
                GSTIN         = t.GSTIN,
                IsActive      = t.IsActive
            }).ToListAsync();
    }

    public async Task<(bool Success, string Message)> CreateTransporterAsync(
        CreateTransporterDto dto, string createdBy)
    {
        _context.Transporters.Add(new Transporter
        {
            Name          = dto.Name.Trim(),
            ContactPerson = dto.ContactPerson?.Trim(),
            Phone         = dto.Phone?.Trim(),
            GSTIN         = dto.GSTIN?.Trim(),
            IsActive      = true,
            CreatedBy     = createdBy,
            CreatedAt     = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();
        return (true, "Transporter created.");
    }

    // ── CHALLANS ──────────────────────────────────────────────────────────
    public async Task<List<DeliveryChallanDto>> GetAllChallansAsync()
    {
        return await _context.DeliveryChallans
            .Include(c => c.SalesOrder)
                .ThenInclude(s => s.Customer)
            .Include(c => c.Transporter)
            .Include(c => c.Shipment)
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new DeliveryChallanDto
            {
                Id              = c.Id,
                ChallanNo       = c.ChallanNo,
                ChallanDate     = c.ChallanDate,
                SONumber        = c.SalesOrder.SONumber,
                CustomerName    = c.SalesOrder.Customer.Name,
                TransporterName = c.Transporter != null ? c.Transporter.Name : null,
                VehicleNumber   = c.VehicleNumber,
                LRNumber        = c.LRNumber,
                TotalValue      = c.TotalValue,
                Status          = c.Status,
                ShipmentStatus  = c.Shipment != null
                    ? c.Shipment.CurrentStatus.ToString()
                    : null
            }).ToListAsync();
    }

    public async Task<DeliveryChallanDto?> GetChallanByIdAsync(int id)
    {
        var challan = await _context.DeliveryChallans
            .Include(c => c.SalesOrder)
                .ThenInclude(s => s.Customer)
            .Include(c => c.Transporter)
            .Include(c => c.Shipment)
            .Include(c => c.Items)
                .ThenInclude(i => i.Item)
                    .ThenInclude(i => i!.UoM)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (challan == null) return null;

        return new DeliveryChallanDto
        {
            Id              = challan.Id,
            ChallanNo       = challan.ChallanNo,
            ChallanDate     = challan.ChallanDate,
            SONumber        = challan.SalesOrder.SONumber,
            CustomerName    = challan.SalesOrder.Customer.Name,
            TransporterName = challan.Transporter?.Name,
            VehicleNumber   = challan.VehicleNumber,
            LRNumber        = challan.LRNumber,
            EWayBillNo      = challan.EWayBillNo,
            DeliveryAddress = challan.DeliveryAddress,
            TotalValue      = challan.TotalValue,
            Status          = challan.Status,
            ShipmentStatus  = challan.Shipment?.CurrentStatus.ToString(),
            Items = challan.Items.Select(i => new ChallanItemDto
            {
                SOItemId      = i.SOItemId,
                ItemId        = i.ItemId,
                ItemCode      = i.Item?.ItemCode ?? "",
                ItemName      = i.Item?.Name ?? "",
                UoMCode       = i.Item?.UoM?.Code,
                DispatchedQty = i.DispatchedQty,
                UnitRate      = i.UnitRate
            }).ToList()
        };
    }

    // KEY FIX — include ALL SO items with pending qty, even those without ItemId
    public async Task<List<ChallanItemDto>> GetSOItemsForChallanAsync(int soId)
    {
        var soItems = await _context.SOItems
            .Include(s => s.Item)
                .ThenInclude(i => i!.UoM)
            .Where(s => s.SalesOrderId == soId)
            .ToListAsync();

        return soItems
            .Select(s => new ChallanItemDto
            {
                SOItemId      = s.Id,
                ItemId        = s.ItemId ?? 0,
                ItemCode      = s.Item?.ItemCode ?? "",
                ItemName      = s.Description,   // always use Description as display name
                UoMCode       = s.UoM?.Code ?? s.Item?.UoM?.Code ?? "",
                UoMId         = s.UoMId,
                DispatchedQty = s.OrderedQty - s.DispatchedQty,  // pending qty
                UnitRate      = s.UnitRate
            })
            .Where(i => i.DispatchedQty > 0)     // only items with pending qty
            .ToList();
    }

    public async Task<(bool Success, string Message, int Id)> CreateChallanAsync(
        CreateChallanDto dto, string createdBy)
    {
        if (dto.Items == null || !dto.Items.Any())
            return (false, "Add at least one item.", 0);

        var so = await _context.SalesOrders
            .Include(s => s.Customer)
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == dto.SOId);

        if (so == null) return (false, "Sales Order not found.", 0);

        // Build challan items — validate each one
        var challanItems = new List<ChallanItem>();
        var soItemsMap   = so.Items.ToDictionary(i => i.Id);

        foreach (var item in dto.Items)
        {
            if (!soItemsMap.TryGetValue(item.SOItemId, out var soItem))
                return (false, $"Item {item.SOItemId} does not belong to this Sales Order.", 0);

            if (item.DispatchedQty <= 0)
                return (false, $"Dispatch quantity for '{soItem.Description}' must be greater than 0.", 0);

            var remaining = soItem.OrderedQty - soItem.DispatchedQty;
            if (item.DispatchedQty > remaining)
                return (false, $"Dispatch qty for '{soItem.Description}' exceeds pending qty ({remaining}).", 0);

            // FIX: Validate the ItemId properly so we don't send 0 to the database
            var finalItemId = item.ItemId > 0 ? item.ItemId : soItem.ItemId;
            
            if (finalItemId == null || finalItemId <= 0)
                return (false, $"Cannot dispatch '{soItem.Description}' because it is not linked to a valid Inventory Item.", 0);

            challanItems.Add(new ChallanItem
            {
                SOItemId      = item.SOItemId,
                ItemId        = finalItemId.Value,
                DispatchedQty = item.DispatchedQty,
                UoMId         = item.UoMId ?? soItem.UoMId,
                UnitRate      = item.UnitRate
            });
        }

        var challanNo  = await _numberService.GenerateAsync("DC");
        var totalValue = challanItems.Sum(c => c.DispatchedQty * c.UnitRate);

        var challan = new DeliveryChallan
        {
            ChallanNo       = challanNo,
            SOId            = dto.SOId,
            CustomerId      = so.CustomerId,
            ChallanDate     = DateTime.UtcNow,
            TransporterId   = dto.TransporterId,
            VehicleNumber   = dto.VehicleNumber?.Trim(),
            DriverName      = dto.DriverName?.Trim(),
            LRNumber        = dto.LRNumber?.Trim(),
            EWayBillNo      = dto.EWayBillNo?.Trim(),
            DeliveryAddress = dto.DeliveryAddress?.Trim() ?? so.Customer.ShippingAddress,
            TotalValue      = totalValue,
            Status          = "Prepared",
            CreatedBy       = createdBy,
            CreatedAt       = DateTime.UtcNow,
            Items           = challanItems,
            
            // FIX: Attach the shipment directly to avoid double SaveChangesAsync
            Shipment = new Shipment
            {
                CurrentStatus   = ShipmentStatus.Dispatched,
                StatusUpdatedAt = DateTime.UtcNow,
                StatusUpdatedBy = createdBy
            }
        };

        _context.DeliveryChallans.Add(challan);

        // Update dispatched qty on each SO item
        foreach (var item in dto.Items)
        {
            if (soItemsMap.TryGetValue(item.SOItemId, out var soItem))
                soItem.DispatchedQty += item.DispatchedQty;
        }

        // Update SO status
        var allDispatched = so.Items.All(i => i.DispatchedQty >= i.OrderedQty);
        so.Status    = allDispatched ? SalesOrderStatus.Dispatched : SalesOrderStatus.Dispatched;
        so.UpdatedAt = DateTime.UtcNow;
        so.UpdatedBy = createdBy;

        // FIX: One single save ensures data integrity (no ghost Challans if Shipment fails)
        await _context.SaveChangesAsync();

        return (true, $"Challan {challanNo} created successfully.", challan.Id);
    }

    // ── SHIPMENT ──────────────────────────────────────────────────────────
    public async Task<(bool Success, string Message)> UpdateShipmentStatusAsync(
        UpdateShipmentDto dto, string updatedBy)
    {
        var shipment = await _context.Shipments
            .Include(s => s.Challan)
                .ThenInclude(c => c.SalesOrder)
            .FirstOrDefaultAsync(s => s.ChallanId == dto.ChallanId);

        if (shipment == null) return (false, "Shipment not found.");

        if (!Enum.TryParse<ShipmentStatus>(dto.Status, out var parsed))
            return (false, "Invalid status.");

        shipment.CurrentStatus         = parsed;
        shipment.StatusUpdatedAt       = DateTime.UtcNow;
        shipment.StatusUpdatedBy       = updatedBy;
        shipment.EstimatedDeliveryDate = dto.EstimatedDeliveryDate;
        shipment.ReceiverName          = dto.ReceiverName;
        shipment.DeliveryRemarks       = dto.DeliveryRemarks;

        if (parsed == ShipmentStatus.Delivered)
        {
            shipment.ActualDeliveryDate           = dto.ActualDeliveryDate ?? DateTime.UtcNow;
            shipment.Challan.Status               = "Delivered";
            shipment.Challan.SalesOrder.Status    = SalesOrderStatus.Delivered;
            shipment.Challan.SalesOrder.UpdatedAt = DateTime.UtcNow;
            shipment.Challan.SalesOrder.UpdatedBy = updatedBy;
        }

        await _context.SaveChangesAsync();
        return (true, $"Shipment status updated to {dto.Status}.");
    }
}
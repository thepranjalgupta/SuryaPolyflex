using Microsoft.EntityFrameworkCore;
using SuryaPolyFlex.Application.Common.Interfaces;
using SuryaPolyFlex.Application.Features.PurchaseOrders;
using SuryaPolyFlex.Application.Features.Indents;
using SuryaPolyFlex.Domain.Entities.Core;
using SuryaPolyFlex.Domain.Entities.Procurement;
using SuryaPolyFlex.Domain.Enums;
using SuryaPolyFlex.Infrastructure.Data;

namespace SuryaPolyFlex.Infrastructure.Services;

public class PurchaseOrderService : IPurchaseOrderService
{
    private readonly AppDbContext _context;
    private readonly INumberSequenceService _numberService;
    private readonly IIndentService _indentService;

    public PurchaseOrderService(
        AppDbContext context,
        INumberSequenceService numberService,
        IIndentService indentService)
    {
        _context       = context;
        _numberService = numberService;
        _indentService = indentService;
    }

    public async Task<List<PurchaseOrderDto>> GetAllAsync(string? status = null)
    {
        var query = _context.PurchaseOrders
            .Include(p => p.Vendor)
            .Include(p => p.Indent)
            .Include(p => p.Items)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status) &&
            Enum.TryParse<PurchaseOrderStatus>(status, out var parsed))
            query = query.Where(p => p.Status == parsed);

        var pos = await query
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return pos.Select(p => MapToDto(p)).ToList();
    }

    public async Task<PurchaseOrderDto?> GetByIdAsync(int id)
    {
        var po = await _context.PurchaseOrders
            .Include(p => p.Vendor)
            .Include(p => p.Indent)
            .Include(p => p.Items)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (po == null) return null;

        var itemIds = po.Items.Select(i => i.ItemId).ToList();
        var items   = await _context.Items
            .Include(i => i.UoM)
            .Where(i => itemIds.Contains(i.Id))
            .ToDictionaryAsync(i => i.Id);

        var dto = MapToDto(po);
        dto.Items = po.Items.Select(i => new POItemDto
        {
            Id          = i.Id,
            ItemId      = i.ItemId,
            ItemCode    = items.ContainsKey(i.ItemId) ? items[i.ItemId].ItemCode : "",
            ItemName    = items.ContainsKey(i.ItemId) ? items[i.ItemId].Name : "",
            UoMCode     = items.ContainsKey(i.ItemId) ? items[i.ItemId].UoM.Code : "",
            OrderedQty  = i.OrderedQty,
            ReceivedQty = i.ReceivedQty,
            PendingQty  = i.OrderedQty - i.ReceivedQty,
            UnitPrice   = i.UnitPrice,
            TaxPct      = i.TaxPct,
            LineTotal   = i.OrderedQty * i.UnitPrice,
            Remarks     = i.Remarks
        }).ToList();

        return dto;
    }

    public async Task<(bool Success, string Message, int POId)> CreateAsync(
        CreatePODto dto, string userName)
    {
        if (dto.Items == null || !dto.Items.Any())
            return (false, "Add at least one item.", 0);

        var validItems = dto.Items
            .Where(i => i.ItemId > 0 && i.OrderedQty > 0 && i.UnitPrice > 0)
            .ToList();

        if (!validItems.Any())
            return (false, "Please provide valid items with quantity and price.", 0);

        var poNumber = await _numberService.GenerateAsync("PO");

        var totalAmount = validItems.Sum(i => i.OrderedQty * i.UnitPrice);

        var po = new PurchaseOrder
        {
            PONumber             = poNumber,
            PODate               = DateTime.UtcNow,
            VendorId             = dto.VendorId,
            IndentId             = dto.IndentId,
            Status               = PurchaseOrderStatus.Open,
            ExpectedDeliveryDate = dto.ExpectedDeliveryDate,
            Terms                = dto.Terms,
            Remarks              = dto.Remarks,
            TotalAmount          = totalAmount,
            CreatedBy            = userName,
            CreatedAt            = DateTime.UtcNow,
            Items = validItems.Select(i => new POItem
            {
                ItemId     = i.ItemId,
                OrderedQty = i.OrderedQty,
                UnitPrice  = i.UnitPrice,
                TaxPct     = i.TaxPct,
                Remarks    = i.Remarks,
                CreatedBy  = userName,
                CreatedAt  = DateTime.UtcNow
            }).ToList()
        };

        _context.PurchaseOrders.Add(po);
        await _context.SaveChangesAsync();

        // If linked to indent, mark indent as POGenerated
        if (dto.IndentId.HasValue)
        {
            var indent = await _context.Indents.FindAsync(dto.IndentId.Value);
            if (indent != null)
            {
                indent.Status    = IndentStatus.POGenerated;
                indent.UpdatedAt = DateTime.UtcNow;
                indent.UpdatedBy = userName;
            }
        }

        _context.WorkflowActions.Add(new WorkflowAction
        {
            ModuleCode   = "PO",
            ReferenceId  = po.Id,
            Action       = "Created",
            ActionByName = userName,
            ActionById   = userName,
            ActionAt     = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
        return (true, $"Purchase Order {poNumber} created successfully.", po.Id);
    }

    public async Task<(bool Success, string Message)> ApprovePOAsync(
        int id, string userId, string userName)
    {
        var po = await _context.PurchaseOrders.FindAsync(id);
        if (po == null) return (false, "PO not found.");
        if (po.Status != PurchaseOrderStatus.Open)
            return (false, "Only Open POs can be approved.");

        po.Status       = PurchaseOrderStatus.Approved;
        po.ApprovedById = userId;
        po.ApprovedAt   = DateTime.UtcNow;
        po.UpdatedAt    = DateTime.UtcNow;
        po.UpdatedBy    = userName;

        _context.WorkflowActions.Add(new WorkflowAction
        {
            ModuleCode   = "PO",
            ReferenceId  = po.Id,
            Action       = "Approved",
            ActionById   = userId,
            ActionByName = userName,
            ActionAt     = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
        return (true, "PO approved successfully.");
    }

    public async Task<(bool Success, string Message)> CancelPOAsync(
        int id, string userName)
    {
        var po = await _context.PurchaseOrders.FindAsync(id);
        if (po == null) return (false, "PO not found.");
        if (po.Status == PurchaseOrderStatus.Closed)
            return (false, "Closed POs cannot be cancelled.");

        po.Status    = PurchaseOrderStatus.Cancelled;
        po.UpdatedAt = DateTime.UtcNow;
        po.UpdatedBy = userName;

        await _context.SaveChangesAsync();
        return (true, "PO cancelled.");
    }

    public async Task<List<PurchaseOrderDto>> GetApprovedPendingReceiptAsync()
    {
        var pos = await _context.PurchaseOrders
            .Include(p => p.Vendor)
            .Include(p => p.Items)
            .Where(p => (p.Status == PurchaseOrderStatus.Open ||
                         p.Status == PurchaseOrderStatus.PartialReceived ||
                         p.Status == PurchaseOrderStatus.Approved) &&
                        !string.IsNullOrEmpty(p.ApprovedById))
            .OrderByDescending(p => p.PODate)
            .ToListAsync();

        return pos.Select(p => MapToDto(p)).ToList();
    }

    private static PurchaseOrderDto MapToDto(PurchaseOrder p) => new()
    {
        Id                   = p.Id,
        PONumber             = p.PONumber,
        PODate               = p.PODate,
        VendorId             = p.VendorId,
        VendorName           = p.Vendor?.Name ?? "",
        IndentId             = p.IndentId,
        IndentNumber         = p.Indent?.IndentNumber,
        Status               = p.Status.ToString(),
        ExpectedDeliveryDate = p.ExpectedDeliveryDate,
        Terms                = p.Terms,
        Remarks              = p.Remarks,
        TotalAmount          = p.TotalAmount
    };
}
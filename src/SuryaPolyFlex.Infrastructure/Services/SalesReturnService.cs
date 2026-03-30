using Microsoft.EntityFrameworkCore;
using SuryaPolyFlex.Application.Common.Interfaces;
using SuryaPolyFlex.Application.Features.SalesOrders;
using SuryaPolyFlex.Domain.Entities.Sales;
using SuryaPolyFlex.Domain.Enums;
using SuryaPolyFlex.Infrastructure.Data;

namespace SuryaPolyFlex.Infrastructure.Services;

public class SalesReturnService
{
    private readonly AppDbContext _context;
    private readonly INumberSequenceService _numberService;
    private readonly IStockService _stockService;

    public SalesReturnService(
        AppDbContext context,
        INumberSequenceService numberService,
        IStockService stockService)
    {
        _context       = context;
        _numberService = numberService;
        _stockService  = stockService;
    }

    public async Task<List<SalesReturnDto>> GetAllAsync()
    {
        return await _context.SalesReturns
            .Include(s => s.SalesOrder)
            .Include(s => s.Customer)
            .OrderByDescending(s => s.CreatedAt)
            .Select(s => new SalesReturnDto
            {
                Id               = s.Id,
                ReturnNo         = s.ReturnNo,
                ReturnDate       = s.ReturnDate,
                SONumber         = s.SalesOrder.SONumber,
                CustomerName     = s.Customer.Name,
                Reason           = s.Reason,
                TotalReturnValue = s.TotalReturnValue,
                Status           = s.Status
            }).ToListAsync();
    }

    public async Task<SalesReturnDto?> GetByIdAsync(int id)
    {
        var ret = await _context.SalesReturns
            .Include(s => s.SalesOrder)
            .Include(s => s.Customer)
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (ret == null) return null;

        return new SalesReturnDto
        {
            Id               = ret.Id,
            ReturnNo         = ret.ReturnNo,
            ReturnDate       = ret.ReturnDate,
            SONumber         = ret.SalesOrder.SONumber,
            CustomerName     = ret.Customer.Name,
            Reason           = ret.Reason,
            TotalReturnValue = ret.TotalReturnValue,
            Status           = ret.Status,
            Items = ret.Items.Select(i => new SalesReturnItemDto
            {
                Description  = i.Description,
                ReturnQty    = i.ReturnQty,
                UnitRate     = i.UnitRate,
                ReturnReason = i.ReturnReason
            }).ToList()
        };
    }

    public async Task<(bool Success, string Message, int Id)> CreateAsync(
        CreateSalesReturnDto dto, string createdBy)
    {
        var so = await _context.SalesOrders
            .Include(s => s.Customer)
            .FirstOrDefaultAsync(s => s.Id == dto.SOId);

        if (so == null) return (false, "Sales Order not found.", 0);

        if (!dto.Items.Any())
            return (false, "Add at least one item.", 0);

        var returnNo    = await _numberService.GenerateAsync("SO");
        var totalValue  = dto.Items.Sum(i => i.ReturnQty * i.UnitRate);

        var ret = new SalesReturn
        {
            ReturnNo         = $"RET-{returnNo}",
            SOId             = dto.SOId,
            CustomerId       = so.CustomerId,
            ReturnDate       = DateTime.UtcNow,
            Reason           = dto.Reason,
            TotalReturnValue = totalValue,
            Status           = "Pending",
            Remarks          = dto.Remarks,
            CreatedBy        = createdBy,
            CreatedAt        = DateTime.UtcNow,
            Items = dto.Items.Select(i => new SalesReturnItem
            {
                ItemId       = i.ItemId,
                Description  = i.Description,
                ReturnQty    = i.ReturnQty,
                UoMId        = i.UoMId,
                UnitRate     = i.UnitRate,
                ReturnReason = i.ReturnReason,
                CreatedBy    = createdBy,
                CreatedAt    = DateTime.UtcNow
            }).ToList()
        };

        _context.SalesReturns.Add(ret);
        await _context.SaveChangesAsync();
        return (true, $"Sales Return {ret.ReturnNo} created.", ret.Id);
    }

    public async Task<(bool Success, string Message)> ApproveAsync(
        int id, int warehouseId, string approvedBy)
    {
        var ret = await _context.SalesReturns
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (ret == null) return (false, "Sales Return not found.");
        if (ret.Status != "Pending") return (false, "Already processed.");

        ret.Status       = "Restocked";
        ret.ApprovedById = approvedBy;
        ret.UpdatedAt    = DateTime.UtcNow;
        ret.UpdatedBy    = approvedBy;

        await _context.SaveChangesAsync();

        // Return stock for items with ItemId
        foreach (var item in ret.Items.Where(i => i.ItemId.HasValue))
        {
            await _stockService.ReceiveStockAsync(
                itemId:          item.ItemId!.Value,
                warehouseId:     warehouseId,
                qty:             item.ReturnQty,
                unitCost:        item.UnitRate,
                transactionType: StockTransactionType.SalesReturn,
                referenceType:   "SalesReturn",
                referenceId:     ret.Id,
                referenceNumber: ret.ReturnNo,
                createdBy:       approvedBy
            );
        }

        return (true, "Sales return approved and stock updated.");
    }
}
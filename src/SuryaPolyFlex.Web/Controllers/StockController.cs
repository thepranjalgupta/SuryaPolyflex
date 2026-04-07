using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SuryaPolyFlex.Application.Common;
using SuryaPolyFlex.Application.Features.Warehouses;
using SuryaPolyFlex.Infrastructure.Data;
using SuryaPolyFlex.Web.Filters;

namespace SuryaPolyFlex.Web.Controllers;

[Authorize]
public class StockController : Controller
{
    private readonly AppDbContext     _context;
    private readonly IWarehouseService _warehouseService;

    public StockController(AppDbContext context, IWarehouseService warehouseService)
    {
        _context          = context;
        _warehouseService = warehouseService;
    }

    public async Task<IActionResult> Balance(int? warehouseId, string? search)
    {
        var warehouses = await _warehouseService.GetAllAsync();
        ViewBag.Warehouses   = new SelectList(warehouses, "Id", "Name");
        ViewBag.WarehouseId  = warehouseId;
        ViewBag.Search       = search;

        var query = _context.StockBalances
            .Include(s => s.Item)
                .ThenInclude(i => i.Category)
            .Include(s => s.Item)
                .ThenInclude(i => i.UoM)
            .Include(s => s.Warehouse)
            .AsQueryable();

        if (warehouseId.HasValue)
            query = query.Where(s => s.WarehouseId == warehouseId);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(s =>
                s.Item.Name.Contains(search) ||
                s.Item.ItemCode.Contains(search));

        var balances = await query
            .OrderBy(s => s.Item.Name)
            .Select(s => new
            {
                s.Item.ItemCode,
                s.Item.Name,
                Category  = s.Item.Category.Name,
                UoM       = s.Item.UoM.Code,
                Warehouse = s.Warehouse.Name,
                s.OnHandQty,
                s.ReservedQty,
                AvailableQty = s.OnHandQty - s.ReservedQty,
                s.Item.MinStockLevel,
                IsLow = s.OnHandQty <= s.Item.MinStockLevel
            })
            .ToListAsync();

        return View(balances);
    }

    public async Task<IActionResult> Ledger(int? itemId, int? warehouseId,
        DateTime? from, DateTime? to)
    {
        var warehouses = await _warehouseService.GetAllAsync();
        ViewBag.Warehouses  = new SelectList(warehouses, "Id", "Name");
        ViewBag.ItemId      = itemId;
        ViewBag.WarehouseId = warehouseId;
        ViewBag.From        = from?.ToString("yyyy-MM-dd");
        ViewBag.To          = to?.ToString("yyyy-MM-dd");

        var query = _context.StockLedgers
            .Include(s => s.Item)
            .Include(s => s.Warehouse)
            .AsQueryable();

        if (itemId.HasValue)
            query = query.Where(s => s.ItemId == itemId);

        if (warehouseId.HasValue)
            query = query.Where(s => s.WarehouseId == warehouseId);

        if (from.HasValue)
            query = query.Where(s => s.CreatedAt >= from.Value);

        if (to.HasValue)
            query = query.Where(s => s.CreatedAt <= to.Value.AddDays(1));

        var ledger = await query
            .OrderByDescending(s => s.CreatedAt)
            .Take(500)
            .Select(s => new
            {
                Date            = s.CreatedAt,
                s.Item.ItemCode,
                ItemName        = s.Item.Name,
                Warehouse       = s.Warehouse.Name,
                TransactionType = s.TransactionType.ToString(),
                s.ReferenceType,
                s.ReferenceNumber,
                s.InwardQty,
                s.OutwardQty,
                s.BalanceQty,
                s.UnitCost,
                s.CreatedBy
            })
            .ToListAsync();

        return View(ledger);
    }
}
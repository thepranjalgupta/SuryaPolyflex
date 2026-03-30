using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SuryaPolyFlex.Domain.Enums;
using SuryaPolyFlex.Infrastructure.Data;

namespace SuryaPolyFlex.Web.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly AppDbContext _context;

    public DashboardController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        // Widget counts
        ViewBag.OpenIndents = await _context.Indents
            .CountAsync(i => i.Status == IndentStatus.PendingApproval);

        ViewBag.OpenSOs = await _context.SalesOrders
            .CountAsync(s => s.Status == SalesOrderStatus.Open ||
                             s.Status == SalesOrderStatus.InProduction);

        ViewBag.ActiveJobCards = await _context.JobCards
            .CountAsync(j => j.Status == Domain.Enums.JobCardStatus.InProgress ||
                             j.Status == Domain.Enums.JobCardStatus.Created);

        ViewBag.PendingDispatches = await _context.SalesOrders
            .CountAsync(s => s.Status == SalesOrderStatus.ReadyToDispatch);

        // Pending indents for approval
        ViewBag.PendingIndents = await _context.Indents
            .Where(i => i.Status == IndentStatus.PendingApproval)
            .OrderByDescending(i => i.CreatedAt)
            .Take(5)
            .Select(i => new { i.IndentNumber, i.IndentDate })
            .ToListAsync();

        // Low stock items
        ViewBag.LowStockItems = await _context.StockBalances
            .Include(s => s.Item)
            .Where(s => s.OnHandQty <= s.Item.MinStockLevel && s.Item.MinStockLevel > 0)
            .Take(5)
            .Select(s => new
            {
                s.Item.Name,
                s.OnHandQty,
                s.Item.MinStockLevel
            })
            .ToListAsync();

        return View();
    }
}
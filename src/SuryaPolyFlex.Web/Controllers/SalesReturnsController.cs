using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SuryaPolyFlex.Application.Common;
using SuryaPolyFlex.Application.Features.SalesOrders;
using SuryaPolyFlex.Application.Features.Warehouses;
using SuryaPolyFlex.Infrastructure.Services;
using SuryaPolyFlex.Web.Filters;

namespace SuryaPolyFlex.Web.Controllers;

[Authorize]
public class SalesReturnsController : Controller
{
    private readonly SalesReturnService _service;
    private readonly ISalesOrderService _soService;
    private readonly IWarehouseService  _warehouseService;

    public SalesReturnsController(
        SalesReturnService service,
        ISalesOrderService soService,
        IWarehouseService warehouseService)
    {
        _service          = service;
        _soService        = soService;
        _warehouseService = warehouseService;
    }

    public async Task<IActionResult> Index()
        => View(await _service.GetAllAsync());

    public async Task<IActionResult> Create(int? soId)
    {
        await LoadDropdownsAsync();
        var dto = new CreateSalesReturnDto { SOId = soId ?? 0 };
        if (soId.HasValue && soId.Value > 0)
        {
            var so = await _soService.GetByIdAsync(soId.Value);
            if (so != null)
            {
                dto.Items = so.Items.Select(i => new CreateSalesReturnItemDto
                {
                    ItemId = i.ItemId,
                    Description = i.Description,
                    ReturnQty = 0, // User will set this
                    UoMId = null, // Will be set by user if needed
                    UnitRate = i.UnitRate
                }).ToList();
            }
        }
        return View(dto);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateSalesReturnDto dto)
    {
        ModelState.Remove("Items");
        if (!ModelState.IsValid)
        {
            await LoadDropdownsAsync();
            return View(dto);
        }

        if (dto.Items == null || !dto.Items.Any())
        {
            ModelState.AddModelError("", "Add at least one item.");
            await LoadDropdownsAsync();
            return View(dto);
        }

        var (success, message, id) =
            await _service.CreateAsync(dto, User.Identity!.Name!);

        if (!success)
        {
            ModelState.AddModelError("", message);
            await LoadDropdownsAsync();
            return View(dto);
        }

        TempData["Success"] = message;
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet]
    public async Task<IActionResult> GetSOItems(int soId)
    {
        var so = await _soService.GetByIdAsync(soId);
        if (so == null) return NotFound();

        var items = so.Items.Select(i => new
        {
            ItemId = i.ItemId,
            Description = i.Description,
            OrderedQty = i.OrderedQty,
            UoMId = (int?)null, // Not available in DTO
            UnitRate = i.UnitRate
        }).ToList();

        return Json(items);
    }

    public async Task<IActionResult> Details(int id)
    {
        var ret = await _service.GetByIdAsync(id);
        if (ret == null) return NotFound();
        var warehouses = await _warehouseService.GetAllAsync();
        ViewBag.Warehouses = new SelectList(warehouses, "Id", "Name");
        return View(ret);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(int id, int warehouseId)
    {
        var (success, message) =
            await _service.ApproveAsync(id, warehouseId, User.Identity!.Name!);
        TempData[success ? "Success" : "Error"] = message;
        return RedirectToAction(nameof(Details), new { id });
    }

    private async Task LoadDropdownsAsync()
    {
        // Allow selection from all active SOs so return creation is flexible
        var allSOs = await _soService.GetAllAsync();
        ViewBag.SalesOrders = new SelectList(allSOs, "Id", "SONumber");
    }
}
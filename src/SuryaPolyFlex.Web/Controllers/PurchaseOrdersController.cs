using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using SuryaPolyFlex.Application.Common;
using SuryaPolyFlex.Application.Features.Items;
using SuryaPolyFlex.Application.Features.PurchaseOrders;
using SuryaPolyFlex.Application.Features.Vendors;
using SuryaPolyFlex.Application.Features.Indents;
using SuryaPolyFlex.Web.Filters;

namespace SuryaPolyFlex.Web.Controllers;

[Authorize]
public class PurchaseOrdersController : Controller
{
    private readonly IPurchaseOrderService _poService;
    private readonly IVendorService        _vendorService;
    private readonly IItemService          _itemService;
    private readonly IIndentService        _indentService;

    public PurchaseOrdersController(
        IPurchaseOrderService poService,
        IVendorService vendorService,
        IItemService itemService,
        IIndentService indentService)
    {
        _poService     = poService;
        _vendorService = vendorService;
        _itemService   = itemService;
        _indentService = indentService;
    }

    [RequirePermission(Permissions.PurchaseOrders.View)]
    public async Task<IActionResult> Index(string? status)
    {
        ViewBag.Status = status;
        return View(await _poService.GetAllAsync(status));
    }

    [RequirePermission(Permissions.PurchaseOrders.Create)]
    public async Task<IActionResult> Create(int? indentId)
    {
        await LoadDropdownsAsync();
        var dto = new CreatePODto { IndentId = indentId };
        if (indentId.HasValue)
        {
            var indent = await _indentService.GetByIdAsync(indentId.Value);
            if (indent != null)
            {
                dto.Items = indent.Items.Select(i => new CreatePOItemDto
                {
                    ItemId = i.ItemId,
                    OrderedQty = i.ApprovedQty,
                    UnitPrice = 0,
                    TaxPct = 0,
                    Remarks = i.Remarks
                }).ToList();
            }
        }
        return View(dto);
    }

    [HttpPost, ValidateAntiForgeryToken]
    [RequirePermission(Permissions.PurchaseOrders.Create)]
    public async Task<IActionResult> Create(CreatePODto dto)
    {
        ModelState.Remove("Items");
        if (!ModelState.IsValid)
        {
            await LoadDropdownsAsync();
            return View(dto);
        }

        if (dto.Items == null || !dto.Items.Any())
        {
            ModelState.AddModelError("", "Please add at least one item.");
            await LoadDropdownsAsync();
            return View(dto);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var userName = User.Identity!.Name!;
        var (success, message, poId) =
            await _poService.CreateAsync(dto, userId, userName);

        if (!success)
        {
            ModelState.AddModelError("", message);
            await LoadDropdownsAsync();
            return View(dto);
        }

        TempData["Success"] = message;
        return RedirectToAction(nameof(Details), new { id = poId });
    }

    [RequirePermission(Permissions.PurchaseOrders.View)]
    public async Task<IActionResult> Details(int id)
    {
        var po = await _poService.GetByIdAsync(id);
        if (po == null) return NotFound();
        return View(po);
    }

    [HttpPost, ValidateAntiForgeryToken]
    [RequirePermission(Permissions.PurchaseOrders.Approve)]
    public async Task<IActionResult> Approve(int id)
    {
        var userId   = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var userName = User.Identity!.Name!;
        var (success, message) =
            await _poService.ApprovePOAsync(id, userId, userName);
        TempData[success ? "Success" : "Error"] = message;
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    [RequirePermission(Permissions.PurchaseOrders.Edit)]
    public async Task<IActionResult> Cancel(int id)
    {
        var (success, message) =
            await _poService.CancelPOAsync(id, User.Identity!.Name!);
        TempData[success ? "Success" : "Error"] = message;
        return RedirectToAction(nameof(Details), new { id });
    }

    private async Task LoadDropdownsAsync()
    {
        var vendors = await _vendorService.GetAllAsync();
        var items   = await _itemService.GetSelectListAsync();
        var indents = await _indentService.GetAllAsync("Approved");
        ViewBag.Vendors = new SelectList(vendors, "Id", "Name");
        ViewBag.Items   = items;
        ViewBag.Indents = new SelectList(indents, "Id", "IndentNumber");
    }
}
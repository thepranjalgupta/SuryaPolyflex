using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SuryaPolyFlex.Application.Common;
using SuryaPolyFlex.Application.Features.GRN;
using SuryaPolyFlex.Application.Features.PurchaseOrders;
using SuryaPolyFlex.Application.Features.Warehouses;
using SuryaPolyFlex.Web.Filters;

namespace SuryaPolyFlex.Web.Controllers;

[Authorize]
public class GRNController : Controller
{
    private readonly IGRNService           _grnService;
    private readonly IPurchaseOrderService _poService;
    private readonly IWarehouseService     _warehouseService;

    public GRNController(
        IGRNService grnService,
        IPurchaseOrderService poService,
        IWarehouseService warehouseService)
    {
        _grnService       = grnService;
        _poService        = poService;
        _warehouseService = warehouseService;
    }

    [RequirePermission(Permissions.GRN.View)]
    public async Task<IActionResult> Index()
        => View(await _grnService.GetAllAsync());

    [RequirePermission(Permissions.GRN.Create)]
    public async Task<IActionResult> Create(int? poId)
    {
        await LoadDropdownsAsync();

        var vm = new CreateGRNViewModel();

        if (poId.HasValue)
        {
            vm.Form.PurchaseOrderId = poId.Value;

            // Load PO items as display lines
            var poItems = await _grnService.GetItemsForPOAsync(poId.Value);
            vm.Lines = poItems.Select(i => new GRNLineItem
            {
                POItemId   = i.Id,
                ItemId     = i.ItemId,
                ItemCode   = i.ItemCode,
                ItemName   = i.ItemName,
                UoMCode    = i.UoMCode,
                PendingQty = i.ReceivedQty,
                UnitCost   = i.UnitCost
            }).ToList();
        }

        return View(vm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    [RequirePermission(Permissions.GRN.Create)]
    public async Task<IActionResult> Create(CreateGRNDto dto)
    {
        ModelState.Remove("Items");

        if (!ModelState.IsValid)
        {
            await LoadDropdownsAsync();
            var vm = new CreateGRNViewModel { Form = dto };
            return View(vm);
        }

        if (dto.Items == null || !dto.Items.Any())
        {
            ModelState.AddModelError("", "Please add at least one item.");
            await LoadDropdownsAsync();
            return View(new CreateGRNViewModel { Form = dto });
        }

        var userName = User.Identity!.Name!;
        var (success, message, grnId) =
            await _grnService.CreateAsync(dto, userName);

        if (!success)
        {
            ModelState.AddModelError("", message);
            await LoadDropdownsAsync();
            return View(new CreateGRNViewModel { Form = dto });
        }

        TempData["Success"] = message;
        return RedirectToAction(nameof(Details), new { id = grnId });
    }

    [RequirePermission(Permissions.GRN.View)]
    public async Task<IActionResult> Details(int id)
    {
        var grn = await _grnService.GetByIdAsync(id);
        if (grn == null) return NotFound();
        return View(grn);
    }

    [HttpPost, ValidateAntiForgeryToken]
    [RequirePermission(Permissions.GRN.Edit)]
    public async Task<IActionResult> Accept(int id)
    {
        var (success, message) =
            await _grnService.AcceptAsync(id, User.Identity!.Name!);
        TempData[success ? "Success" : "Error"] = message;
        return RedirectToAction(nameof(Details), new { id });
    }

    private async Task LoadDropdownsAsync()
    {
        var pos        = await _poService.GetApprovedPendingReceiptAsync();
        var warehouses = await _warehouseService.GetAllAsync();
        ViewBag.PurchaseOrders = new SelectList(pos, "Id", "PONumber");
        ViewBag.Warehouses     = new SelectList(warehouses, "Id", "Name");
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SuryaPolyFlex.Application.Features.Dispatch;
using SuryaPolyFlex.Application.Features.SalesOrders;

namespace SuryaPolyFlex.Web.Controllers;

[Authorize]
public class DispatchController : Controller
{
    private readonly IDispatchService   _dispatchService;
    private readonly ISalesOrderService _soService;

    public DispatchController(
        IDispatchService dispatchService,
        ISalesOrderService soService)
    {
        _dispatchService = dispatchService;
        _soService       = soService;
    }

    // ── TRANSPORTERS ──────────────────────────────────────────────────────
    public async Task<IActionResult> Transporters()
        => View(await _dispatchService.GetTransportersAsync());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateTransporter(CreateTransporterDto dto)
    {
        var (success, message) =
            await _dispatchService.CreateTransporterAsync(dto, User.Identity!.Name!);
        TempData[success ? "Success" : "Error"] = message;
        return RedirectToAction(nameof(Transporters));
    }

    // ── CHALLANS ──────────────────────────────────────────────────────────
    public async Task<IActionResult> Challans()
        => View(await _dispatchService.GetAllChallansAsync());

    public async Task<IActionResult> CreateChallan(int? soId)
    {
        await LoadChallanDropdownsAsync();

        var dto = new CreateChallanDto();

        if (soId.HasValue)
        {
            dto.SOId  = soId.Value;
            var items = await _dispatchService.GetSOItemsForChallanAsync(soId.Value);
            dto.Items = items.Select(i => new CreateChallanItemDto
            {
                SOItemId      = i.SOItemId,
                ItemId        = i.ItemId,
                ItemName      = i.ItemName,
                UoMCode       = i.UoMCode,
                UoMId         = i.UoMId,
                DispatchedQty = i.DispatchedQty,
                UnitRate      = i.UnitRate
            }).ToList();
        }

        return View(dto);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateChallan(CreateChallanDto dto)
    {
        // Remove Items from model state — we validate manually
        ModelState.Remove("Items");

        if (!ModelState.IsValid)
        {
            await LoadChallanDropdownsAsync();
            return View(dto);
        }

        if (dto.Items == null || !dto.Items.Any())
        {
            ModelState.AddModelError("", "Please select a Sales Order and load items before saving.");
            await LoadChallanDropdownsAsync();
            return View(dto);
        }

        var (success, message, id) =
            await _dispatchService.CreateChallanAsync(dto, User.Identity!.Name!);

        if (!success)
        {
            ModelState.AddModelError("", message);
            await LoadChallanDropdownsAsync();
            return View(dto);
        }

        TempData["Success"] = message;
        return RedirectToAction(nameof(ChallanDetails), new { id });
    }

    // AJAX endpoint — returns JSON for SO items
    [HttpGet]
    public async Task<IActionResult> GetSOItemsForChallan(int soId)
    {
        if (soId <= 0)
            return Json(new List<object>());

        var items = await _dispatchService.GetSOItemsForChallanAsync(soId);
        return Json(items);
    }

    public async Task<IActionResult> ChallanDetails(int id)
    {
        var challan = await _dispatchService.GetChallanByIdAsync(id);
        if (challan == null) return NotFound();
        return View(challan);
    }

    // ── SHIPMENT ──────────────────────────────────────────────────────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateShipment(UpdateShipmentDto dto)
    {
        var (success, message) =
            await _dispatchService.UpdateShipmentStatusAsync(dto, User.Identity!.Name!);
        TempData[success ? "Success" : "Error"] = message;
        return RedirectToAction(nameof(ChallanDetails), new { id = dto.ChallanId });
    }

    private async Task LoadChallanDropdownsAsync()
    {
        var transporters = await _dispatchService.GetTransportersAsync();
        var readySOs     = await _soService.GetAllAsync("ReadyToDispatch");
        var inProdSOs    = await _soService.GetAllAsync("InProduction");
        var openSOs      = await _soService.GetAllAsync("Open");
        var allSOs       = readySOs.Concat(inProdSOs).Concat(openSOs).ToList();

        ViewBag.Transporters = new SelectList(transporters, "Id", "Name");
        ViewBag.SalesOrders  = new SelectList(allSOs, "Id", "SONumber");
    }
}
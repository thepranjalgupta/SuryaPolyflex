using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SuryaPolyFlex.Application.Common;
using SuryaPolyFlex.Application.Features.GateEntry;
using SuryaPolyFlex.Application.Features.PurchaseOrders;
using SuryaPolyFlex.Application.Features.Vendors;
using SuryaPolyFlex.Infrastructure.Services;
using SuryaPolyFlex.Web.Filters;

namespace SuryaPolyFlex.Web.Controllers;

[Authorize]
public class GateEntryController : Controller
{
    private readonly GateEntryService     _service;
    private readonly IVendorService       _vendorService;
    private readonly IPurchaseOrderService _poService;

    public GateEntryController(
        GateEntryService service,
        IVendorService vendorService,
        IPurchaseOrderService poService)
    {
        _service       = service;
        _vendorService = vendorService;
        _poService     = poService;
    }

    public async Task<IActionResult> Index()
        => View(await _service.GetAllAsync());

    public async Task<IActionResult> Create()
    {
        await LoadDropdownsAsync();
        return View(new CreateGateEntryDto());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateGateEntryDto dto)
    {
        if (!ModelState.IsValid)
        {
            await LoadDropdownsAsync();
            return View(dto);
        }

        var (success, message, _) =
            await _service.CreateAsync(dto, User.Identity!.Name!);

        TempData[success ? "Success" : "Error"] = message;
        return success
            ? RedirectToAction(nameof(Index))
            : View(dto);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkExit(int id)
    {
        var (success, message) = await _service.MarkExitAsync(id);
        TempData[success ? "Success" : "Error"] = message;
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadDropdownsAsync()
    {
        var vendors = await _vendorService.GetAllAsync();
        var pos     = await _poService.GetApprovedPendingReceiptAsync();
        ViewBag.Vendors        = new SelectList(vendors, "Id", "Name");
        ViewBag.PurchaseOrders = new SelectList(pos, "Id", "PONumber");
    }
}
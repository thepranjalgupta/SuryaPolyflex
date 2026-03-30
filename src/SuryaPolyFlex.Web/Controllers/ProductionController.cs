using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SuryaPolyFlex.Application.Features.Production;
using SuryaPolyFlex.Application.Features.Items;
using SuryaPolyFlex.Application.Features.SalesOrders;

namespace SuryaPolyFlex.Web.Controllers;

[Authorize]
public class ProductionController : Controller
{
    private readonly IProductionService _service;
    private readonly IItemService       _itemService;
    private readonly ISalesOrderService _soService;

    public ProductionController(
        IProductionService service,
        IItemService itemService,
        ISalesOrderService soService)
    {
        _service     = service;
        _itemService = itemService;
        _soService   = soService;
    }

    // ── MACHINES ──────────────────────────────────────────────────────────
    public async Task<IActionResult> Machines()
        => View(await _service.GetMachinesAsync());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateMachine(CreateMachineDto dto)
    {
        var (success, message) =
            await _service.CreateMachineAsync(dto, User.Identity!.Name!);
        TempData[success ? "Success" : "Error"] = message;
        return RedirectToAction(nameof(Machines));
    }

    // ── JOB CARDS ─────────────────────────────────────────────────────────
    public async Task<IActionResult> JobCards(string? status)
    {
        ViewBag.Status = status;
        return View(await _service.GetJobCardsAsync(status));
    }

    public async Task<IActionResult> CreateJobCard()
    {
        await LoadJobCardDropdownsAsync();
        return View(new CreateJobCardDto
        {
            PlannedStartDate = DateTime.Today,
            PlannedEndDate   = DateTime.Today.AddDays(7)
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateJobCard(CreateJobCardDto dto)
    {
        if (!ModelState.IsValid)
        {
            await LoadJobCardDropdownsAsync();
            return View(dto);
        }

        var (success, message, id) =
            await _service.CreateJobCardAsync(dto, User.Identity!.Name!);

        if (!success)
        {
            ModelState.AddModelError("", message);
            await LoadJobCardDropdownsAsync();
            return View(dto);
        }

        TempData["Success"] = message;
        return RedirectToAction(nameof(JobCardDetails), new { id });
    }

    public async Task<IActionResult> JobCardDetails(int id)
    {
        var jc = await _service.GetJobCardByIdAsync(id);
        if (jc == null) return NotFound();
        return View(jc);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateJobCardStatus(int id, string status)
    {
        var (success, message) =
            await _service.UpdateJobCardStatusAsync(id, status, User.Identity!.Name!);
        TempData[success ? "Success" : "Error"] = message;
        return RedirectToAction(nameof(JobCardDetails), new { id });
    }

    // ── BOM ───────────────────────────────────────────────────────────────
    public async Task<IActionResult> CreateBOM(int jobCardId)
    {
        var items = await _itemService.GetSelectListAsync();
        ViewBag.Items = items;
        return View(new CreateBOMDto { JobCardId = jobCardId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateBOM(CreateBOMDto dto)
    {
        ModelState.Remove("Items");
        if (!ModelState.IsValid)
        {
            ViewBag.Items = await _itemService.GetSelectListAsync();
            return View(dto);
        }

        if (dto.Items == null || !dto.Items.Any())
        {
            ModelState.AddModelError("", "Add at least one item.");
            ViewBag.Items = await _itemService.GetSelectListAsync();
            return View(dto);
        }

        var (success, message, _) =
            await _service.CreateBOMAsync(dto, User.Identity!.Name!);

        TempData[success ? "Success" : "Error"] = message;
        return RedirectToAction(nameof(JobCardDetails), new { id = dto.JobCardId });
    }

    // ── WORK ORDERS ───────────────────────────────────────────────────────
    public async Task<IActionResult> WorkOrders(string? status)
    {
        ViewBag.Status = status;
        return View(await _service.GetWorkOrdersAsync(status));
    }

    public async Task<IActionResult> CreateWorkOrder(int jobCardId)
    {
        var machines = await _service.GetMachinesAsync();
        ViewBag.Machines = new SelectList(machines, "Id", "Name");
        ViewBag.Shifts   = new SelectList(new[] { "Morning", "Afternoon", "Night" });
        return View(new CreateWorkOrderDto { JobCardId = jobCardId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateWorkOrder(CreateWorkOrderDto dto)
    {
        if (!ModelState.IsValid)
        {
            var machines = await _service.GetMachinesAsync();
            ViewBag.Machines = new SelectList(machines, "Id", "Name");
            ViewBag.Shifts   = new SelectList(new[] { "Morning", "Afternoon", "Night" });
            return View(dto);
        }

        var (success, message, id) =
            await _service.CreateWorkOrderAsync(dto, User.Identity!.Name!);

        TempData[success ? "Success" : "Error"] = message;
        return success
            ? RedirectToAction(nameof(WorkOrderDetails), new { id })
            : RedirectToAction(nameof(JobCardDetails), new { id = dto.JobCardId });
    }

    public async Task<IActionResult> WorkOrderDetails(int id)
    {
        var wo = await _service.GetWorkOrderByIdAsync(id);
        if (wo == null) return NotFound();
        return View(wo);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateWorkOrderStatus(int id, string status)
    {
        var (success, message) =
            await _service.UpdateWorkOrderStatusAsync(id, status, User.Identity!.Name!);
        TempData[success ? "Success" : "Error"] = message;
        return RedirectToAction(nameof(WorkOrderDetails), new { id });
    }

    // ── PRODUCTION ENTRIES ────────────────────────────────────────────────
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AddProductionEntry(CreateProductionEntryDto dto)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Invalid entry data.";
            return RedirectToAction(nameof(WorkOrderDetails), new { id = dto.WorkOrderId });
        }

        var (success, message) =
            await _service.AddProductionEntryAsync(dto, User.Identity!.Name!);

        TempData[success ? "Success" : "Error"] = message;
        return RedirectToAction(nameof(WorkOrderDetails), new { id = dto.WorkOrderId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CompleteWorkOrder(int id, int fgWarehouseId)
    {
        var (success, message) =
            await _service.CompleteWorkOrderAsync(id, fgWarehouseId, User.Identity!.Name!);
        TempData[success ? "Success" : "Error"] = message;
        return RedirectToAction(nameof(WorkOrderDetails), new { id });
    }

    private async Task LoadJobCardDropdownsAsync()
    {
        var machines = await _service.GetMachinesAsync();
        var sos      = await _soService.GetAllAsync("InProduction");
        var openSOs  = await _soService.GetAllAsync("Open");
        var allSOs   = sos.Concat(openSOs).ToList();

        ViewBag.Machines = new SelectList(machines, "Id", "Name");
        ViewBag.SalesOrders = new SelectList(allSOs, "Id", "SONumber");
        ViewBag.Shifts   = new SelectList(new[] { "Morning", "Afternoon", "Night" });
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SuryaPolyFlex.Application.Common;
using SuryaPolyFlex.Application.Features.MaterialIssue;
using SuryaPolyFlex.Application.Features.Items;
using SuryaPolyFlex.Application.Features.Warehouses;
using SuryaPolyFlex.Web.Filters;

namespace SuryaPolyFlex.Web.Controllers;

[Authorize]
public class MaterialIssueController : Controller
{
    private readonly IMaterialIssueService _service;
    private readonly IItemService          _itemService;
    private readonly IWarehouseService     _warehouseService;

    public MaterialIssueController(
        IMaterialIssueService service,
        IItemService itemService,
        IWarehouseService warehouseService)
    {
        _service          = service;
        _itemService      = itemService;
        _warehouseService = warehouseService;
    }

    public async Task<IActionResult> Index()
        => View(await _service.GetAllAsync());

    public async Task<IActionResult> Create()
    {
        await LoadDropdownsAsync();
        return View(new CreateMaterialIssueDto());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateMaterialIssueDto dto)
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

    public async Task<IActionResult> Details(int id)
    {
        var issue = await _service.GetByIdAsync(id);
        if (issue == null) return NotFound();
        return View(issue);
    }

    private async Task LoadDropdownsAsync()
    {
        var warehouses  = await _warehouseService.GetAllAsync();
        var items       = await _itemService.GetSelectListAsync();
        ViewBag.Warehouses = new SelectList(warehouses, "Id", "Name");
        ViewBag.Items      = items;
    }
}
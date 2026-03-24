using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SuryaPolyFlex.Application.Features.Warehouses;
using SuryaPolyFlex.Web.Filters;
using SuryaPolyFlex.Application.Common;

namespace SuryaPolyFlex.Web.Controllers;

[Authorize]
public class WarehousesController : Controller
{
    private readonly IWarehouseService _service;
    public WarehousesController(IWarehouseService service) => _service = service;

    public async Task<IActionResult> Index()
        => View(await _service.GetAllAsync());

    public IActionResult Create()
    {
        LoadWarehouseTypes();
        return View(new CreateWarehouseDto());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateWarehouseDto dto)
    {
        if (!ModelState.IsValid) { LoadWarehouseTypes(); return View(dto); }
        var (success, message) = await _service.CreateAsync(dto, User.Identity!.Name!);
        if (!success) { ModelState.AddModelError("", message); LoadWarehouseTypes(); return View(dto); }
        TempData["Success"] = message;
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var w = await _service.GetByIdAsync(id);
        if (w == null) return NotFound();
        LoadWarehouseTypes();
        return View(new EditWarehouseDto
        {
            Id = w.Id, Code = w.Code, Name = w.Name,
            Location = w.Location, WarehouseType = w.WarehouseType,
            IsActive = w.IsActive
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditWarehouseDto dto)
    {
        if (!ModelState.IsValid) { LoadWarehouseTypes(); return View(dto); }
        var (success, message) = await _service.UpdateAsync(dto, User.Identity!.Name!);
        if (!success) { ModelState.AddModelError("", message); LoadWarehouseTypes(); return View(dto); }
        TempData["Success"] = message;
        return RedirectToAction(nameof(Index));
    }

    private void LoadWarehouseTypes() =>
        ViewBag.WarehouseTypes = new SelectList(new[] { "RM", "FG", "WIP", "Scrap" });
}
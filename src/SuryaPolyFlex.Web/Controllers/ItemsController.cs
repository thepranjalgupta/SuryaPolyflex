using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SuryaPolyFlex.Application.Common;
using SuryaPolyFlex.Application.Features.Items;
using SuryaPolyFlex.Web.Filters;

namespace SuryaPolyFlex.Web.Controllers;

[Authorize]
public class ItemsController : Controller
{
    private readonly IItemService _service;
    public ItemsController(IItemService service) => _service = service;

    [RequirePermission(Permissions.Items.View)]
    public async Task<IActionResult> Index(string? search, string? itemType)
    {
        ViewBag.Search   = search;
        ViewBag.ItemType = itemType;
        ViewBag.ItemTypes = new SelectList(
            new[] { "RM", "FG", "WIP", "Consumable" });
        return View(await _service.GetAllAsync(search, itemType));
    }

    [RequirePermission(Permissions.Items.Create)]
    public async Task<IActionResult> Create()
    {
        await LoadDropdownsAsync();
        return View(new CreateItemDto());
    }

    [HttpPost, ValidateAntiForgeryToken]
    [RequirePermission(Permissions.Items.Create)]
    public async Task<IActionResult> Create(CreateItemDto dto)
    {
        if (!ModelState.IsValid) { await LoadDropdownsAsync(); return View(dto); }
        var (success, message) = await _service.CreateAsync(dto, User.Identity!.Name!);
        if (!success) { ModelState.AddModelError("", message); await LoadDropdownsAsync(); return View(dto); }
        TempData["Success"] = message;
        return RedirectToAction(nameof(Index));
    }

    [RequirePermission(Permissions.Items.Edit)]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null) return NotFound();
        await LoadDropdownsAsync();
        return View(new EditItemDto
        {
            Id = item.Id, ItemCode = item.ItemCode, Name = item.Name,
            Description = item.Description, CategoryId = item.CategoryId,
            UoMId = item.UoMId, ItemType = item.ItemType,
            MinStockLevel = item.MinStockLevel, ReorderQty = item.ReorderQty,
            StandardCost = item.StandardCost, IsActive = item.IsActive
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    [RequirePermission(Permissions.Items.Edit)]
    public async Task<IActionResult> Edit(EditItemDto dto)
    {
        if (!ModelState.IsValid) { await LoadDropdownsAsync(); return View(dto); }
        var (success, message) = await _service.UpdateAsync(dto, User.Identity!.Name!);
        if (!success) { ModelState.AddModelError("", message); await LoadDropdownsAsync(); return View(dto); }
        TempData["Success"] = message;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    [RequirePermission(Permissions.Items.Delete)]
    public async Task<IActionResult> Delete(int id)
    {
        var (success, message) = await _service.DeleteAsync(id);
        TempData[success ? "Success" : "Error"] = message;
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadDropdownsAsync()
    {
        var categories = await _service.GetCategoriesAsync();
        var uoms       = await _service.GetUoMsAsync();
        ViewBag.Categories = new SelectList(categories.Select(c =>
            new { c.Item1, Display = $"{c.Item2} - {c.Item3}" }), "Item1", "Display");
        ViewBag.UoMs = new SelectList(uoms.Select(u =>
            new { u.Item1, Display = $"{u.Item2} - {u.Item3}" }), "Item1", "Display");
        ViewBag.ItemTypes = new SelectList(new[] { "RM", "FG", "WIP", "Consumable" });
    }
}
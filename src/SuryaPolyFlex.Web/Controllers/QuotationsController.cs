using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SuryaPolyFlex.Application.Common;
using SuryaPolyFlex.Application.Features.Customers;
using SuryaPolyFlex.Application.Features.Items;
using SuryaPolyFlex.Application.Features.Quotations;
using SuryaPolyFlex.Web.Filters;

namespace SuryaPolyFlex.Web.Controllers;

[Authorize]
[RequirePermission(Permissions.Quotations.View)]
    public class QuotationsController : Controller
{
    private readonly IQuotationService _service;
    private readonly ICustomerService  _customerService;
    private readonly IItemService      _itemService;

    public QuotationsController(
        IQuotationService service,
        ICustomerService customerService,
        IItemService itemService)
    {
        _service         = service;
        _customerService = customerService;
        _itemService     = itemService;
    }

    [RequirePermission(Permissions.Quotations.View)]
    public async Task<IActionResult> Index(string? status)
    {
        ViewBag.Status = status;
        return View(await _service.GetAllAsync(status));
    }

    public async Task<IActionResult> Create()
    {
        await LoadDropdownsAsync();
        return View(new CreateQuotationDto());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateQuotationDto dto)
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
        var q = await _service.GetByIdAsync(id);
        if (q == null) return NotFound();
        return View(q);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, string status)
    {
        var (success, message) =
            await _service.UpdateStatusAsync(id, status, User.Identity!.Name!);
        TempData[success ? "Success" : "Error"] = message;
        return RedirectToAction(nameof(Details), new { id });
    }

    private async Task LoadDropdownsAsync()
    {
        var customers = await _customerService.GetAllAsync();
        var items     = await _itemService.GetSelectListAsync();
        ViewBag.Customers = new SelectList(customers, "Id", "Name");
        ViewBag.Items     = items;
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuryaPolyFlex.Application.Common;
using SuryaPolyFlex.Application.Features.Vendors;
using SuryaPolyFlex.Web.Filters;

namespace SuryaPolyFlex.Web.Controllers;

[Authorize]
public class VendorsController : Controller
{
    private readonly IVendorService _service;
    public VendorsController(IVendorService service) => _service = service;

    [RequirePermission(Permissions.Vendors.View)]
    public async Task<IActionResult> Index(string? search)
    {
        ViewBag.Search = search;
        return View(await _service.GetAllAsync(search));
    }

    [RequirePermission(Permissions.Vendors.Create)]
    public IActionResult Create() => View(new CreateVendorDto());

    [HttpPost, ValidateAntiForgeryToken]
    [RequirePermission(Permissions.Vendors.Create)]
    public async Task<IActionResult> Create(CreateVendorDto dto)
    {
        if (!ModelState.IsValid) return View(dto);
        var (success, message) = await _service.CreateAsync(dto, User.Identity!.Name!);
        if (!success) { ModelState.AddModelError("", message); return View(dto); }
        TempData["Success"] = message;
        return RedirectToAction(nameof(Index));
    }

    [RequirePermission(Permissions.Vendors.Edit)]
    public async Task<IActionResult> Edit(int id)
    {
        var vendor = await _service.GetByIdAsync(id);
        if (vendor == null) return NotFound();
        return View(new EditVendorDto
        {
            Id = vendor.Id, VendorCode = vendor.VendorCode,
            Name = vendor.Name, ContactPerson = vendor.ContactPerson,
            Email = vendor.Email, Mobile = vendor.Mobile,
            City = vendor.City, State = vendor.State,
            GSTIN = vendor.GSTIN, PaymentTermDays = vendor.PaymentTermDays,
            IsActive = vendor.IsActive
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    [RequirePermission(Permissions.Vendors.Edit)]
    public async Task<IActionResult> Edit(EditVendorDto dto)
    {
        if (!ModelState.IsValid) return View(dto);
        var (success, message) = await _service.UpdateAsync(dto, User.Identity!.Name!);
        if (!success) { ModelState.AddModelError("", message); return View(dto); }
        TempData["Success"] = message;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    [RequirePermission(Permissions.Vendors.Delete)]
    public async Task<IActionResult> Delete(int id)
    {
        var (success, message) = await _service.DeleteAsync(id);
        TempData[success ? "Success" : "Error"] = message;
        return RedirectToAction(nameof(Index));
    }
}
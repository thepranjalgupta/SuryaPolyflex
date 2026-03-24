using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuryaPolyFlex.Application.Common;
using SuryaPolyFlex.Application.Features.Customers;
using SuryaPolyFlex.Web.Filters;

namespace SuryaPolyFlex.Web.Controllers;

[Authorize]
public class CustomersController : Controller
{
    private readonly ICustomerService _service;
    public CustomersController(ICustomerService service) => _service = service;

    [RequirePermission(Permissions.Customers.View)]
    public async Task<IActionResult> Index(string? search)
    {
        ViewBag.Search = search;
        return View(await _service.GetAllAsync(search));
    }

    [RequirePermission(Permissions.Customers.Create)]
    public IActionResult Create() => View(new CreateCustomerDto());

    [HttpPost, ValidateAntiForgeryToken]
    [RequirePermission(Permissions.Customers.Create)]
    public async Task<IActionResult> Create(CreateCustomerDto dto)
    {
        if (!ModelState.IsValid) return View(dto);
        var (success, message) = await _service.CreateAsync(dto, User.Identity!.Name!);
        if (!success) { ModelState.AddModelError("", message); return View(dto); }
        TempData["Success"] = message;
        return RedirectToAction(nameof(Index));
    }

    [RequirePermission(Permissions.Customers.Edit)]
    public async Task<IActionResult> Edit(int id)
    {
        var customer = await _service.GetByIdAsync(id);
        if (customer == null) return NotFound();
        return View(new EditCustomerDto
        {
            Id = customer.Id, CustomerCode = customer.CustomerCode,
            Name = customer.Name, ContactPerson = customer.ContactPerson,
            Email = customer.Email, Mobile = customer.Mobile,
            City = customer.City, State = customer.State,
            GSTIN = customer.GSTIN, CreditLimit = customer.CreditLimit,
            PaymentTermDays = customer.PaymentTermDays, IsActive = customer.IsActive
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    [RequirePermission(Permissions.Customers.Edit)]
    public async Task<IActionResult> Edit(EditCustomerDto dto)
    {
        if (!ModelState.IsValid) return View(dto);
        var (success, message) = await _service.UpdateAsync(dto, User.Identity!.Name!);
        if (!success) { ModelState.AddModelError("", message); return View(dto); }
        TempData["Success"] = message;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    [RequirePermission(Permissions.Customers.Delete)]
    public async Task<IActionResult> Delete(int id)
    {
        var (success, message) = await _service.DeleteAsync(id);
        TempData[success ? "Success" : "Error"] = message;
        return RedirectToAction(nameof(Index));
    }
}
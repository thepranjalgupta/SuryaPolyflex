using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SuryaPolyFlex.Application.Features.Customers;
using SuryaPolyFlex.Application.Features.Leads;
using SuryaPolyFlex.Application.Common;
using SuryaPolyFlex.Web.Filters;

namespace SuryaPolyFlex.Web.Controllers;

[Authorize]
[RequirePermission(Permissions.Leads.View)]
    public class LeadsController : Controller
{
    private readonly ILeadService     _leadService;
    private readonly ICustomerService _customerService;

    public LeadsController(ILeadService leadService, ICustomerService customerService)
    {
        _leadService     = leadService;
        _customerService = customerService;
    }

    [RequirePermission(Permissions.Leads.View)]
    public async Task<IActionResult> Index(string? status)
    {
        ViewBag.Status = status;
        return View(await _leadService.GetAllAsync(status));
    }

    [RequirePermission(Permissions.Leads.Create)]
    public async Task<IActionResult> Create()
    {
        await LoadCustomersAsync();
        return View(new CreateLeadDto());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateLeadDto dto)
    {
        if (!ModelState.IsValid)
        {
            await LoadCustomersAsync();
            return View(dto);
        }
        var (success, message) =
            await _leadService.CreateAsync(dto, User.Identity!.Name!);
        if (!success)
        {
            ModelState.AddModelError("", message);
            await LoadCustomersAsync();
            return View(dto);
        }
        TempData["Success"] = message;
        return RedirectToAction(nameof(Index));
    }

    [RequirePermission(Permissions.Leads.Edit)]
    public async Task<IActionResult> Edit(int id)
    {
        var lead = await _leadService.GetByIdAsync(id);
        if (lead == null) return NotFound();
        await LoadCustomersAsync();
        ViewBag.Statuses = new SelectList(new[]
        {
            "New","Contacted","Qualified","QuotationSent","Converted","Lost"
        });
        return View(new EditLeadDto
        {
            Id = lead.Id, Title = lead.Title, CustomerId = lead.CustomerId,
            ContactPerson = lead.ContactPerson, Phone = lead.Phone,
            Email = lead.Email, Source = lead.Source, Status = lead.Status,
            FollowUpDate = lead.FollowUpDate, Remarks = lead.Remarks
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditLeadDto dto)
    {
        if (!ModelState.IsValid)
        {
            await LoadCustomersAsync();
            return View(dto);
        }
        var (success, message) =
            await _leadService.UpdateAsync(dto, User.Identity!.Name!);
        TempData[success ? "Success" : "Error"] = message;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    [RequirePermission(Permissions.Leads.Convert)]
    public async Task<IActionResult> Convert(int id)
    {
        var (success, message) =
            await _leadService.ConvertToCustomerAsync(id, User.Identity!.Name!);
        TempData[success ? "Success" : "Error"] = message;
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadCustomersAsync()
    {
        var customers = await _customerService.GetAllAsync();
        ViewBag.Customers = new SelectList(customers, "Id", "Name");
    }
}
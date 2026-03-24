using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuryaPolyFlex.Application.Common;
using SuryaPolyFlex.Application.Features.Departments;
using SuryaPolyFlex.Web.Filters;

namespace SuryaPolyFlex.Web.Controllers;

[Authorize]
public class DepartmentsController : Controller
{
    private readonly IDepartmentService _service;

    public DepartmentsController(IDepartmentService service)
    {
        _service = service;
    }

    [RequirePermission(Permissions.Departments.View)]
    public async Task<IActionResult> Index(string? search)
    {
        var departments = await _service.GetAllAsync(search);
        ViewBag.Search = search;
        return View(departments);
    }

    [RequirePermission(Permissions.Departments.Create)]
    public IActionResult Create() => View(new CreateDepartmentDto());

    [HttpPost, ValidateAntiForgeryToken]
    [RequirePermission(Permissions.Departments.Create)]
    public async Task<IActionResult> Create(CreateDepartmentDto dto)
    {
        if (!ModelState.IsValid) return View(dto);

        var (success, message) = await _service.CreateAsync(dto, User.Identity!.Name!);

        if (!success)
        {
            ModelState.AddModelError("", message);
            return View(dto);
        }

        TempData["Success"] = message;
        return RedirectToAction(nameof(Index));
    }

    [RequirePermission(Permissions.Departments.Edit)]
    public async Task<IActionResult> Edit(int id)
    {
        var dept = await _service.GetByIdAsync(id);
        if (dept == null) return NotFound();

        return View(new EditDepartmentDto
        {
            Id          = dept.Id,
            Code        = dept.Code,
            Name        = dept.Name,
            Description = dept.Description,
            IsActive    = dept.IsActive
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    [RequirePermission(Permissions.Departments.Edit)]
    public async Task<IActionResult> Edit(EditDepartmentDto dto)
    {
        if (!ModelState.IsValid) return View(dto);

        var (success, message) = await _service.UpdateAsync(dto, User.Identity!.Name!);

        if (!success)
        {
            ModelState.AddModelError("", message);
            return View(dto);
        }

        TempData["Success"] = message;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    [RequirePermission(Permissions.Departments.Delete)]
    public async Task<IActionResult> Delete(int id)
    {
        var (success, message) = await _service.DeleteAsync(id);
        TempData[success ? "Success" : "Error"] = message;
        return RedirectToAction(nameof(Index));
    }
}
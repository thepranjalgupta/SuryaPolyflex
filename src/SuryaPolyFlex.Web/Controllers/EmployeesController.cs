using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SuryaPolyFlex.Application.Common;
using SuryaPolyFlex.Application.Common.Interfaces;
using SuryaPolyFlex.Application.Features.Departments;
using SuryaPolyFlex.Application.Features.Employees;
using SuryaPolyFlex.Domain.Entities.Core;
using SuryaPolyFlex.Web.Filters;

namespace SuryaPolyFlex.Web.Controllers;

[Authorize]
public class EmployeesController : Controller
{
    private readonly IEmployeeService _employeeService;
    private readonly IDepartmentService _departmentService;
    private readonly IPermissionService _permissionService;
    private readonly UserManager<ApplicationUser> _userManager;

    public EmployeesController(
        IEmployeeService employeeService,
        IDepartmentService departmentService,
        IPermissionService permissionService,
        UserManager<ApplicationUser> userManager)
    {
        _employeeService   = employeeService;
        _departmentService = departmentService;
        _permissionService = permissionService;
        _userManager       = userManager;
    }

    [RequirePermission(Permissions.Employees.View)]
    public async Task<IActionResult> Index(string? search)
    {
        var employees = await _employeeService.GetAllAsync(search);
        ViewBag.Search = search;
        return View(employees);
    }

    [RequirePermission(Permissions.Employees.Create)]
    public async Task<IActionResult> Create()
    {
        await LoadDepartmentsAsync();
        return View(new CreateEmployeeDto());
    }

    [HttpPost, ValidateAntiForgeryToken]
    [RequirePermission(Permissions.Employees.Create)]
    public async Task<IActionResult> Create(CreateEmployeeDto dto)
    {
        if (!ModelState.IsValid)
        {
            await LoadDepartmentsAsync();
            return View(dto);
        }

        var (success, message) = await _employeeService.CreateAsync(dto, User.Identity!.Name!);

        if (!success)
        {
            ModelState.AddModelError("", message);
            await LoadDepartmentsAsync();
            return View(dto);
        }

        TempData["Success"] = message;
        return RedirectToAction(nameof(Index));
    }

    [RequirePermission(Permissions.Employees.Edit)]
    public async Task<IActionResult> Edit(int id)
    {
        var emp = await _employeeService.GetByIdAsync(id);
        if (emp == null) return NotFound();

        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(userId))
        {
            var appUser = await _userManager.FindByIdAsync(userId);
            var userDepartmentId = appUser?.DepartmentId;

            var hasDynamicPermission = await _permissionService.HasPermissionAsync(userId, Permissions.Employees.Edit, new Dictionary<string, object>
            {
                ["resource.DepartmentId"] = emp.DepartmentId,
                ["user.DepartmentId"] = userDepartmentId
            });

            if (!hasDynamicPermission)
                return Forbid();
        }

        await LoadDepartmentsAsync();
        return View(new EditEmployeeDto
        {
            Id           = emp.Id,
            EmployeeCode = emp.EmployeeCode,
            FullName     = emp.FullName,
            Email        = emp.Email,
            Phone        = emp.Phone,
            Designation  = emp.Designation,
            DepartmentId = emp.DepartmentId,
            IsActive     = emp.IsActive
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    [RequirePermission(Permissions.Employees.Edit)]
    public async Task<IActionResult> Edit(EditEmployeeDto dto)
    {
        if (!ModelState.IsValid)
        {
            await LoadDepartmentsAsync();
            return View(dto);
        }

        var (success, message) = await _employeeService.UpdateAsync(dto, User.Identity!.Name!);

        if (!success)
        {
            ModelState.AddModelError("", message);
            await LoadDepartmentsAsync();
            return View(dto);
        }

        TempData["Success"] = message;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    [RequirePermission(Permissions.Employees.Delete)]
    public async Task<IActionResult> Delete(int id)
    {
        var (success, message) = await _employeeService.DeleteAsync(id);
        TempData[success ? "Success" : "Error"] = message;
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadDepartmentsAsync()
    {
        var departments = await _departmentService.GetAllAsync();
        ViewBag.Departments = new SelectList(departments, "Id", "Name");
    }
}
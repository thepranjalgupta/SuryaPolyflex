using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SuryaPolyFlex.Application.Common;
using SuryaPolyFlex.Application.Features.Departments;
using SuryaPolyFlex.Application.Features.Employees;
using SuryaPolyFlex.Application.Features.Users;
using SuryaPolyFlex.Web.Filters;

namespace SuryaPolyFlex.Web.Controllers;

[Authorize]
public class UsersController : Controller
{
    private readonly IUserService _userService;
    private readonly IDepartmentService _departmentService;
    private readonly IEmployeeService _employeeService;

    public UsersController(
        IUserService userService,
        IDepartmentService departmentService,
        IEmployeeService employeeService)
    {
        _userService       = userService;
        _departmentService = departmentService;
        _employeeService   = employeeService;
    }

    [RequirePermission(Permissions.Users.View)]
    public async Task<IActionResult> Index(string? search)
    {
        var users = await _userService.GetAllAsync(search);
        ViewBag.Search = search;
        return View(users);
    }

    [RequirePermission(Permissions.Users.Create)]
    public async Task<IActionResult> Create()
    {
        await LoadDropdownsAsync();
        return View(new CreateUserDto());
    }

    [HttpPost, ValidateAntiForgeryToken]
    [RequirePermission(Permissions.Users.Create)]
    public async Task<IActionResult> Create(CreateUserDto dto)
    {
        if (!ModelState.IsValid)
        {
            await LoadDropdownsAsync();
            return View(dto);
        }

        var (success, message) = await _userService.CreateAsync(dto, User.Identity!.Name!);

        if (!success)
        {
            ModelState.AddModelError("", message);
            await LoadDropdownsAsync();
            return View(dto);
        }

        TempData["Success"] = message;
        return RedirectToAction(nameof(Index));
    }

    [RequirePermission(Permissions.Users.Edit)]
    public async Task<IActionResult> Edit(string id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null) return NotFound();

        await LoadDropdownsAsync();
        return View(new EditUserDto
        {
            Id           = user.Id,
            FullName     = user.FullName,
            Phone        = user.Phone,
            RoleName     = user.Roles.FirstOrDefault() ?? "",
            IsActive     = user.IsActive
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    [RequirePermission(Permissions.Users.Edit)]
    public async Task<IActionResult> Edit(EditUserDto dto)
    {
        if (!ModelState.IsValid)
        {
            await LoadDropdownsAsync();
            return View(dto);
        }

        var (success, message) = await _userService.UpdateAsync(dto);

        if (!success)
        {
            ModelState.AddModelError("", message);
            await LoadDropdownsAsync();
            return View(dto);
        }

        TempData["Success"] = message;
        return RedirectToAction(nameof(Index));
    }

    [RequirePermission(Permissions.Users.Edit)]
    public async Task<IActionResult> ResetPassword(string id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null) return NotFound();

        ViewBag.UserName = user.FullName;
        return View(new ResetPasswordDto { UserId = id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    [RequirePermission(Permissions.Users.Edit)]
    public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        var (success, message) = await _userService.ResetPasswordAsync(dto);

        if (!success)
        {
            ModelState.AddModelError("", message);
            return View(dto);
        }

        TempData["Success"] = message;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    [RequirePermission(Permissions.Users.Edit)]
    public async Task<IActionResult> ToggleActive(string id)
    {
        var (success, message) = await _userService.ToggleActiveAsync(id);
        TempData[success ? "Success" : "Error"] = message;
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadDropdownsAsync()
    {
        var roles       = await _userService.GetAllRolesAsync();
        var departments = await _departmentService.GetAllAsync();
        var employees   = await _employeeService.GetAllAsync();

        ViewBag.Roles       = new SelectList(roles);
        ViewBag.Departments = new SelectList(departments, "Id", "Name");
        ViewBag.Employees   = new SelectList(employees, "Id", "FullName");
    }
}
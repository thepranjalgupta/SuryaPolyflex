using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SuryaPolyFlex.Application.Common;
using SuryaPolyFlex.Application.Common.Interfaces;
using SuryaPolyFlex.Application.Features.Departments;
using SuryaPolyFlex.Application.Features.Employees;
using SuryaPolyFlex.Application.Features.Users;
using SuryaPolyFlex.Domain.Enums;
using SuryaPolyFlex.Web.Filters;

namespace SuryaPolyFlex.Web.Controllers;

[Authorize]
public class UsersController : Controller
{
    private readonly IUserService _userService;
    private readonly IDepartmentService _departmentService;
    private readonly IEmployeeService _employeeService;
    private readonly IPermissionService _permissionService;

    public UsersController(
        IUserService userService,
        IDepartmentService departmentService,
        IEmployeeService employeeService,
        IPermissionService permissionService)
    {
        _userService       = userService;
        _departmentService = departmentService;
        _employeeService   = employeeService;
        _permissionService = permissionService;
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
            Id            = user.Id,
            FullName      = user.FullName,
            Phone         = user.Phone,
            RoleName      = user.Roles.FirstOrDefault() ?? "",
            DepartmentId  = user.DepartmentId,
            AuthorityRole = user.AuthorityRole,
            EmployeeId    = user.EmployeeId,
            IsActive      = user.IsActive
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

    // ── Permission Override Management ────────────────────────────────
    
    [RequirePermission(Permissions.Users.Edit)]
    public async Task<IActionResult> UserPermissionOverrides(string userId)
    {
        var user = await _userService.GetByIdAsync(userId);
        if (user == null) return NotFound();

        var overrides = await _permissionService.GetUserPermissionOverridesAsync(userId);
        var allPermissions = await _permissionService.GetAllPermissionsAsync();

        ViewBag.UserId = userId;
        ViewBag.UserName = user.FullName;
        ViewBag.AllPermissions = allPermissions;

        return View(overrides);
    }

    [HttpPost, ValidateAntiForgeryToken]
    [RequirePermission(Permissions.Users.Edit)]
    public async Task<IActionResult> GrantUserPermission(string userId, int permissionId, int? expiryDays = null)
    {
        var user = await _userService.GetByIdAsync(userId);
        if (user == null) return NotFound();

        try
        {
            var expiryDate = expiryDays.HasValue && expiryDays > 0
                ? DateTime.Now.AddDays(expiryDays.Value)
                : (DateTime?)null;

            await _permissionService.GrantUserPermissionAsync(
                userId,
                permissionId,
                User.Identity?.Name ?? "SYSTEM",
                expiryDate);

            TempData["Success"] = "Permission granted successfully.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(UserPermissionOverrides), new { userId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    [RequirePermission(Permissions.Users.Edit)]
    public async Task<IActionResult> DenyUserPermission(string userId, int permissionId, int? expiryDays = null)
    {
        var user = await _userService.GetByIdAsync(userId);
        if (user == null) return NotFound();

        try
        {
            var expiryDate = expiryDays.HasValue && expiryDays > 0
                ? DateTime.Now.AddDays(expiryDays.Value)
                : (DateTime?)null;

            await _permissionService.DenyUserPermissionAsync(
                userId,
                permissionId,
                User.Identity?.Name ?? "SYSTEM",
                expiryDate);

            TempData["Success"] = "Permission denied successfully.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(UserPermissionOverrides), new { userId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    [RequirePermission(Permissions.Users.Edit)]
    public async Task<IActionResult> RevokeUserPermission(string userId, int permissionId)
    {
        var user = await _userService.GetByIdAsync(userId);
        if (user == null) return NotFound();

        var success = await _permissionService.RevokeUserPermissionAsync(userId, permissionId);

        if (success)
            TempData["Success"] = "Permission override revoked successfully.";
        else
            TempData["Error"] = "Failed to revoke permission override.";

        return RedirectToAction(nameof(UserPermissionOverrides), new { userId });
    }

    private async Task LoadDropdownsAsync()
    {
        var roles       = await _userService.GetAllRolesAsync();
        var departments = await _departmentService.GetAllAsync();
        var employees   = await _employeeService.GetAllAsync();
        var permissions = await _permissionService.GetAllPermissionsAsync();

        ViewBag.Roles           = new SelectList(roles);
        ViewBag.Departments     = new SelectList(departments, "Id", "Name");
        ViewBag.Employees       = new SelectList(employees, "Id", "FullName");
        ViewBag.AuthorityRoles  = new SelectList(Enum.GetValues(typeof(AuthorityRole))
            .Cast<AuthorityRole>()
            .Select(r => new { Value = r, Text = r.ToString() }), "Value", "Text");
        ViewBag.AllPermissions  = permissions.OrderBy(p => p.Module).ThenBy(p => p.Action).ToList();
    }
}
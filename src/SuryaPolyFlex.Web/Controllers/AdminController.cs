using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SuryaPolyFlex.Application.Common.Interfaces;
using SuryaPolyFlex.Domain.Entities.Core;
using SuryaPolyFlex.Infrastructure.Data;

namespace SuryaPolyFlex.Web.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IPermissionService _permissionService;
    private readonly AppDbContext _context;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public AdminController(
        IPermissionService permissionService,
        AppDbContext context,
        RoleManager<ApplicationRole> roleManager)
    {
        _permissionService = permissionService;
        _context = context;
        _roleManager = roleManager;
    }

    // Default action - show RBAC dashboard
    public IActionResult Index() => View("Dashboard");

    // Permissions - List all
    public async Task<IActionResult> Permissions()
    {
        var list = await _permissionService.GetAllPermissionsAsync();
        return View(list);
    }

    // Permission - Details
    public async Task<IActionResult> Permission(int id)
    {
        var permission = await _permissionService.GetPermissionByIdAsync(id);
        if (permission == null) return NotFound();
        return View(permission);
    }

    // Create Permission
    [HttpPost]
    public async Task<IActionResult> CreatePermission([FromForm] Permission permission)
    {
        permission.CreatedBy = User.Identity?.Name ?? "SYSTEM";
        await _permissionService.CreatePermissionAsync(permission);
        TempData["Success"] = "Permission created successfully.";
        return RedirectToAction(nameof(Permissions));
    }

    // Update Permission
    [HttpPost]
    public async Task<IActionResult> UpdatePermission([FromForm] Permission permission)
    {
        permission.UpdatedBy = User.Identity?.Name ?? "SYSTEM";
        var success = await _permissionService.UpdatePermissionAsync(permission);
        if (!success) return NotFound();
        TempData["Success"] = "Permission updated successfully.";
        return RedirectToAction(nameof(Permissions));
    }

    // Delete Permission (soft delete)
    [HttpPost]
    public async Task<IActionResult> DeletePermission(int id)
    {
        var success = await _permissionService.DeletePermissionAsync(id);
        if (!success) return NotFound();
        TempData["Success"] = "Permission deleted successfully.";
        return RedirectToAction(nameof(Permissions));
    }

    // Role-Permission assignment
    [HttpGet]
    public async Task<IActionResult> RolePermissions(string roleId)
    {
        var allRoles = await _roleManager.Roles
            .Where(r => r.IsActive)
            .OrderBy(r => r.Name)
            .Select(r => r.Name!)
            .ToListAsync();

        var allPermissions = await _permissionService.GetAllPermissionsAsync();

        ViewBag.AllRoles = allRoles;
        ViewBag.AllPermissions = allPermissions;

        if (string.IsNullOrEmpty(roleId))
        {
            return View(new List<RolePermission>());
        }

        var data = await _context.RolePermissions
            .Include(rp => rp.Permission)
            .Where(rp => rp.Role.Name == roleId && !rp.IsDeleted)
            .ToListAsync();

        return View(data);
    }

    // Grant permission to role
    [HttpPost]
    public async Task<IActionResult> GrantPermission(string roleId, string permissionName)
    {
        if (string.IsNullOrEmpty(roleId) || string.IsNullOrEmpty(permissionName))
        {
            TempData["Error"] = "Invalid role or permission.";
            return RedirectToAction(nameof(RolePermissions));
        }

        await _permissionService.GrantPermissionAsync(roleId, permissionName);
        TempData["Success"] = $"Permission '{permissionName}' granted to role '{roleId}'.";
        return RedirectToAction(nameof(RolePermissions), new { roleId });
    }

    // Revoke permission from role
    [HttpPost]
    public async Task<IActionResult> RevokePermission(string roleId, string permissionName)
    {
        if (string.IsNullOrEmpty(roleId) || string.IsNullOrEmpty(permissionName))
        {
            TempData["Error"] = "Invalid role or permission.";
            return RedirectToAction(nameof(RolePermissions));
        }

        await _permissionService.RevokePermissionAsync(roleId, permissionName);
        TempData["Success"] = $"Permission '{permissionName}' revoked from role '{roleId}'.";
        return RedirectToAction(nameof(RolePermissions), new { roleId });
    }

    // Rules - List by permission
    [HttpGet]
    public async Task<IActionResult> Rules(int permissionId)
    {
        if (permissionId <= 0)
        {
            TempData["Error"] = "Invalid permission selected.";
            return RedirectToAction(nameof(Permissions));
        }

        var rules = await _permissionService.GetRulesByPermissionIdAsync(permissionId);
        ViewBag.PermissionId = permissionId;
        return View(rules);
    }

    // Create Rule
    [HttpPost]
    public async Task<IActionResult> CreateRule([FromForm] Rule rule)
    {
        rule.CreatedBy = User.Identity?.Name ?? "SYSTEM";

        if (rule.PermissionId <= 0)
        {
            TempData["Error"] = "Please select a valid permission.";
            return RedirectToAction(nameof(Permissions));
        }

        try
        {
            await _permissionService.CreateRuleAsync(rule);
            TempData["Success"] = "Rule created successfully.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Permissions));
        }

        return RedirectToAction(nameof(Rules), new { permissionId = rule.PermissionId });
    }

    // Update Rule
    [HttpPost]
    public async Task<IActionResult> UpdateRule([FromForm] Rule rule)
    {
        rule.UpdatedBy = User.Identity?.Name ?? "SYSTEM";
        var success = await _permissionService.UpdateRuleAsync(rule);
        if (!success) return NotFound();
        TempData["Success"] = "Rule updated successfully.";
        return RedirectToAction(nameof(Rules), new { permissionId = rule.PermissionId });
    }

    // Delete Rule (soft delete)
    [HttpPost]
    public async Task<IActionResult> DeleteRule(int id)
    {
        await _permissionService.DeleteRuleAsync(id);
        TempData["Success"] = "Rule deleted successfully.";
        return RedirectToAction(nameof(Permissions));
    }
}

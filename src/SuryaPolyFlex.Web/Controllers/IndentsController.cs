using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using SuryaPolyFlex.Application.Common;
using SuryaPolyFlex.Application.Features.Departments;
using SuryaPolyFlex.Application.Features.Indents;
using SuryaPolyFlex.Application.Features.Items;
using SuryaPolyFlex.Web.Filters;

namespace SuryaPolyFlex.Web.Controllers;

[Authorize]
public class IndentsController : Controller
{
    private readonly IIndentService    _indentService;
    private readonly IDepartmentService _deptService;
    private readonly IItemService      _itemService;

    public IndentsController(
        IIndentService indentService,
        IDepartmentService deptService,
        IItemService itemService)
    {
        _indentService = indentService;
        _deptService   = deptService;
        _itemService   = itemService;
    }

    [RequirePermission(Permissions.Indents.View)]
    public async Task<IActionResult> Index(string? status)
    {
        ViewBag.Status = status;
        ViewBag.Statuses = new SelectList(new[]
        {
            "Draft","PendingApproval","Approved","Rejected","POGenerated"
        });
        return View(await _indentService.GetAllAsync(status));
    }

    [RequirePermission(Permissions.Indents.Create)]
    public async Task<IActionResult> Create()
    {
        await LoadDropdownsAsync();
        return View(new CreateIndentDto());
    }

    [HttpPost, ValidateAntiForgeryToken]
[RequirePermission(Permissions.Indents.Create)]
public async Task<IActionResult> Create(CreateIndentDto dto)
{
    // Remove Items validation from ModelState if empty
    // (handled manually below)
    ModelState.Remove("Items");

    if (!ModelState.IsValid)
    {
        await LoadDropdownsAsync();
        return View(dto);
    }

    if (dto.Items == null || !dto.Items.Any())
    {
        ModelState.AddModelError("", "Please add at least one item.");
        await LoadDropdownsAsync();
        return View(dto);
    }

    var userId   = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    var userName = User.Identity!.Name!;

    var (success, message, indentId) =
        await _indentService.CreateAsync(dto, userId, userName);

    if (!success)
    {
        ModelState.AddModelError("", message);
        await LoadDropdownsAsync();
        return View(dto);
    }

    TempData["Success"] = message;
    return RedirectToAction(nameof(Details), new { id = indentId });
}

    [RequirePermission(Permissions.Indents.View)]
    public async Task<IActionResult> Details(int id)
    {
        var indent = await _indentService.GetByIdAsync(id);
        if (indent == null) return NotFound();
        return View(indent);
    }

    [HttpPost, ValidateAntiForgeryToken]
    [RequirePermission(Permissions.Indents.Create)]
    public async Task<IActionResult> Submit(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var (success, message) = await _indentService.SubmitForApprovalAsync(id, userId);
        TempData[success ? "Success" : "Error"] = message;
        return RedirectToAction(nameof(Details), new { id });
    }

    [RequirePermission(Permissions.Indents.Approve)]
    public async Task<IActionResult> Approve(int id)
    {
        var indent = await _indentService.GetByIdAsync(id);
        if (indent == null) return NotFound();
        return View(indent);
    }

    [HttpPost, ValidateAntiForgeryToken]
    [RequirePermission(Permissions.Indents.Approve)]
    public async Task<IActionResult> Approve(ApproveIndentDto dto)
    {
        var userId   = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var userName = User.Identity!.Name!;
        var (success, message) =
            await _indentService.ApproveOrRejectAsync(dto, userId, userName);
        TempData[success ? "Success" : "Error"] = message;
        return RedirectToAction(nameof(Details), new { id = dto.IndentId });
    }

    private async Task LoadDropdownsAsync()
    {
        var depts = await _deptService.GetAllAsync();
        var items = await _itemService.GetSelectListAsync();
        ViewBag.Departments = new SelectList(depts, "Id", "Name");
        ViewBag.Items       = items;
    }
}
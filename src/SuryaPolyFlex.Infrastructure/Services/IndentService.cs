using Microsoft.EntityFrameworkCore;
using SuryaPolyFlex.Application.Common.Interfaces;
using SuryaPolyFlex.Application.Features.Indents;
using SuryaPolyFlex.Domain.Entities.Core;
using SuryaPolyFlex.Domain.Entities.Procurement;
using SuryaPolyFlex.Domain.Enums;
using SuryaPolyFlex.Infrastructure.Data;

namespace SuryaPolyFlex.Infrastructure.Services;

public class IndentService : IIndentService
{
    private readonly AppDbContext _context;
    private readonly INumberSequenceService _numberService;

    public IndentService(AppDbContext context, INumberSequenceService numberService)
    {
        _context       = context;
        _numberService = numberService;
    }

    public async Task<List<IndentDto>> GetAllAsync(string? status = null)
    {
        var query = _context.Indents
            .Include(i => i.Items)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status) &&
            Enum.TryParse<IndentStatus>(status, out var parsedStatus))
            query = query.Where(i => i.Status == parsedStatus);

        var indents = await query.OrderByDescending(i => i.CreatedAt).ToListAsync();

        var deptIds = indents.Select(i => i.DepartmentId).Distinct().ToList();
        var userIds = indents.Select(i => i.RequestedById).Distinct().ToList();

        var departments = await _context.Departments
            .Where(d => deptIds.Contains(d.Id))
            .ToDictionaryAsync(d => d.Id, d => d.Name);

        var users = await _context.Users
            .Where(u => userIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.FullName);

        return indents.Select(i => new IndentDto
        {
            Id              = i.Id,
            IndentNumber    = i.IndentNumber,
            IndentDate      = i.IndentDate,
            DepartmentName  = departments.GetValueOrDefault(i.DepartmentId, "N/A"),
            RequestedByName = users.GetValueOrDefault(i.RequestedById, "N/A"),
            Status          = i.Status.ToString(),
            Remarks         = i.Remarks,
            Items           = i.Items.Select(it => new IndentItemDto
            {
                Id           = it.Id,
                ItemId       = it.ItemId,
                RequestedQty = it.RequestedQty,
                ApprovedQty  = it.ApprovedQty,
                Remarks      = it.Remarks
            }).ToList()
        }).ToList();
    }

    public async Task<IndentDto?> GetByIdAsync(int id)
    {
        var indent = await _context.Indents
            .Include(i => i.Items)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (indent == null) return null;

        var dept = await _context.Departments.FindAsync(indent.DepartmentId);
        var user = await _context.Users.FindAsync(indent.RequestedById);

        var itemIds = indent.Items.Select(i => i.ItemId).ToList();
        var items = await _context.Items
            .Include(i => i.UoM)
            .Where(i => itemIds.Contains(i.Id))
            .ToDictionaryAsync(i => i.Id);

        return new IndentDto
        {
            Id              = indent.Id,
            IndentNumber    = indent.IndentNumber,
            IndentDate      = indent.IndentDate,
            DepartmentName  = dept?.Name ?? "N/A",
            RequestedByName = user?.FullName ?? "N/A",
            Status          = indent.Status.ToString(),
            Remarks         = indent.Remarks,
            Items           = indent.Items.Select(it => new IndentItemDto
            {
                Id           = it.Id,
                ItemId       = it.ItemId,
                ItemCode     = items.ContainsKey(it.ItemId) ? items[it.ItemId].ItemCode : "",
                ItemName     = items.ContainsKey(it.ItemId) ? items[it.ItemId].Name : "",
                UoMCode      = items.ContainsKey(it.ItemId) ? items[it.ItemId].UoM.Code : "",
                RequestedQty = it.RequestedQty,
                ApprovedQty  = it.ApprovedQty,
                Remarks      = it.Remarks
            }).ToList()
        };
    }

   public async Task<(bool Success, string Message, int IndentId)> CreateAsync(
    CreateIndentDto dto, string userId, string userName)
{
    if (dto.Items == null || !dto.Items.Any())
        return (false, "Add at least one item.", 0);

    // Filter out any rows where ItemId is 0 (blank row submitted)
    var validItems = dto.Items.Where(i => i.ItemId > 0 && i.RequestedQty > 0).ToList();
    if (!validItems.Any())
        return (false, "Please select valid items with quantity.", 0);

    var indentNumber = await _numberService.GenerateAsync("INDENT");

    var indent = new Indent
    {
        IndentNumber  = indentNumber,
        IndentDate    = DateTime.UtcNow,
        DepartmentId  = dto.DepartmentId,
        RequestedById = userId,
        Status        = IndentStatus.Draft,
        Remarks       = dto.Remarks,
        CreatedBy     = userName,
        CreatedAt     = DateTime.UtcNow,
        Items         = validItems.Select(i => new IndentItem
        {
            ItemId       = i.ItemId,
            RequestedQty = i.RequestedQty,
            ApprovedQty  = 0,
            Remarks      = i.Remarks,
            CreatedBy    = userName,
            CreatedAt    = DateTime.UtcNow
        }).ToList()
    };

    _context.Indents.Add(indent);
    await _context.SaveChangesAsync();

    _context.WorkflowActions.Add(new WorkflowAction
    {
        ModuleCode   = "INDENT",
        ReferenceId  = indent.Id,
        Action       = "Created",
        ActionById   = userId,
        ActionByName = userName,
        ActionAt     = DateTime.UtcNow
    });
    await _context.SaveChangesAsync();

    return (true, $"Indent {indentNumber} created successfully.", indent.Id);
}
    public async Task<(bool Success, string Message)> SubmitForApprovalAsync(
        int id, string userId)
    {
        var indent = await _context.Indents.FindAsync(id);
        if (indent == null) return (false, "Indent not found.");
        if (indent.Status != IndentStatus.Draft)
            return (false, "Only Draft indents can be submitted.");
        if (indent.RequestedById != userId)
            return (false, "You can only submit your own indents.");

        indent.Status    = IndentStatus.PendingApproval;
        indent.UpdatedAt = DateTime.UtcNow;
        indent.UpdatedBy = userId;

        _context.WorkflowActions.Add(new WorkflowAction
        {
            ModuleCode  = "INDENT",
            ReferenceId = indent.Id,
            Action      = "SubmittedForApproval",
            ActionById  = userId,
            ActionAt    = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
        return (true, "Indent submitted for approval.");
    }

    public async Task<(bool Success, string Message)> ApproveOrRejectAsync(
        ApproveIndentDto dto, string userId, string userName)
    {
        var indent = await _context.Indents
            .Include(i => i.Items)
            .FirstOrDefaultAsync(i => i.Id == dto.IndentId);

        if (indent == null) return (false, "Indent not found.");
        if (indent.Status != IndentStatus.PendingApproval)
            return (false, "Indent is not pending approval.");

        if (dto.Action == "Approve")
        {
            // Update approved quantities
            foreach (var item in dto.Items)
            {
                var indentItem = indent.Items.FirstOrDefault(i => i.Id == item.IndentItemId);
                if (indentItem != null)
                    indentItem.ApprovedQty = item.ApprovedQty;
            }

            indent.Status      = IndentStatus.Approved;
            indent.ApprovedById = userId;
            indent.ApprovedAt  = DateTime.UtcNow;
        }
        else
        {
            indent.Status = IndentStatus.Rejected;
        }

        indent.UpdatedAt = DateTime.UtcNow;
        indent.UpdatedBy = userId;

        _context.WorkflowActions.Add(new WorkflowAction
        {
            ModuleCode   = "INDENT",
            ReferenceId  = indent.Id,
            Action       = dto.Action == "Approve" ? "Approved" : "Rejected",
            ActionById   = userId,
            ActionByName = userName,
            Remarks      = dto.Remarks,
            ActionAt     = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
        return (true, $"Indent {dto.Action}d successfully.");
    }
}
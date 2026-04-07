using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SuryaPolyFlex.Application.Common;
using SuryaPolyFlex.Application.Common.Interfaces;
using SuryaPolyFlex.Domain.Entities.Core;
using SuryaPolyFlex.Infrastructure.Data;

namespace SuryaPolyFlex.Infrastructure.Services;

public class ApprovalService : IApprovalService
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IPermissionService _permissionService;

    public ApprovalService(AppDbContext context, UserManager<ApplicationUser> userManager, IPermissionService permissionService)
    {
        _context = context;
        _userManager = userManager;
        _permissionService = permissionService;
    }

    public async Task<ApprovalTransaction> CreateApprovalTransactionAsync(string module, int recordId, string creatorId, int? creatorDepartmentId)
    {
        var workflow = await _context.ApprovalWorkflows
            .Include(w => w.Steps)
            .FirstOrDefaultAsync(w => w.Module == module && w.IsActive && !w.IsDeleted);

        if (workflow == null)
            throw new InvalidOperationException($"Approval workflow not found for module '{module}'");

        if (!workflow.Steps.Any())
            throw new InvalidOperationException($"Approval workflow for '{module}' has no steps configured.");

        var roles = await _userManager.GetRolesAsync(await _userManager.FindByIdAsync(creatorId) ?? throw new InvalidOperationException("Creator not found"));

        var currentStepNumber = 1;

        var firstStep = workflow.Steps.OrderBy(s => s.StepOrder).First();
        if (roles.Contains(firstStep.RoleName, StringComparer.OrdinalIgnoreCase))
        {
            var nextStep = workflow.Steps.OrderBy(s => s.StepOrder).Skip(1).FirstOrDefault();
            if (nextStep != null)
                currentStepNumber = nextStep.StepOrder;
        }

        var transaction = new ApprovalTransaction
        {
            Module = module,
            RecordId = recordId,
            CurrentStep = currentStepNumber,
            Status = ApprovalStatus.Pending,
            CreatedBy = creatorId,
            CreatedAt = DateTime.UtcNow
        };

        _context.ApprovalTransactions.Add(transaction);
        await _context.SaveChangesAsync();

        return transaction;
    }

    public async Task<ApprovalTransaction?> GetApprovalTransactionAsync(string module, int recordId)
    {
        return await _context.ApprovalTransactions
            .FirstOrDefaultAsync(t => t.Module == module && t.RecordId == recordId && !t.IsDeleted);
    }

    public async Task<bool> CanApproveAsync(string userId, string module, int currentStep, int? approverDepartmentId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null || !user.IsActive) return false;

        var workflow = await _context.ApprovalWorkflows
            .Include(w => w.Steps)
            .FirstOrDefaultAsync(w => w.Module == module && w.IsActive && !w.IsDeleted);

        if (workflow == null) return false;

        var step = workflow.Steps.FirstOrDefault(s => s.StepOrder == currentStep);
        if (step == null) return false;

        // Get the module approval permission
        var moduleApprovePermission = module switch
        {
            "INDENTS" => Permissions.Indents.Approve,
            "PO" => Permissions.PurchaseOrders.Approve,
            "SO" => Permissions.SalesOrders.Approve,
            "DISPATCH" => Permissions.Dispatch.Create, // no approve constant, fall back to create for dispatch
            _ => null
        };

        // ✅ Check if user has explicit override for this approval permission
        if (!string.IsNullOrEmpty(moduleApprovePermission))
        {
            var hasOverride = await _context.UserPermissions
                .Where(up =>
                    up.UserId == userId &&
                    up.Permission.Name == moduleApprovePermission &&
                    up.IsActive &&
                    up.IsAllowed &&
                    (up.ExpiryDate == null || up.ExpiryDate > DateTime.UtcNow))
                .AnyAsync();

            if (hasOverride)
            {
                // User has explicit override - allow approval regardless of role/department
                return true;
            }
        }

        // ✅ Standard approval flow: Check role and department
        var userRoles = await _userManager.GetRolesAsync(user);
        if (!userRoles.Contains(step.RoleName, StringComparer.OrdinalIgnoreCase))
            return false;

        if (step.IsDepartmentScoped)
        {
            if (!approverDepartmentId.HasValue)
                return false;

            var deptMatch = await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => u.DepartmentId)
                .FirstOrDefaultAsync();

            if (!deptMatch.HasValue || deptMatch.Value != approverDepartmentId.Value)
                return false;
        }

        // Permission check for module approve
        if (!string.IsNullOrEmpty(moduleApprovePermission))
        {
            var hasPermission = await _permissionService.HasPermissionAsync(userId, moduleApprovePermission!);
            if (!hasPermission)
                return false;
        }

        return true;
    }

    public async Task<ApprovalTransaction> ApproveAsync(string userId, string module, int recordId, int currentStep)
    {
        var transaction = await GetApprovalTransactionAsync(module, recordId);
        if (transaction == null) throw new InvalidOperationException("Approval transaction not found.");

        if (transaction.Status != ApprovalStatus.Pending)
            throw new InvalidOperationException("Approval transaction is not pending.");

        if (transaction.CurrentStep != currentStep)
            throw new InvalidOperationException("Invalid step for approval.");

        var workflow = await _context.ApprovalWorkflows.Include(w => w.Steps)
            .FirstOrDefaultAsync(w => w.Module == module && w.IsActive && !w.IsDeleted);

        if (workflow == null) throw new InvalidOperationException("Approval workflow not found.");

        var step = workflow.Steps.FirstOrDefault(s => s.StepOrder == currentStep)
            ?? throw new InvalidOperationException("Approval step not found.");

        if (!await CanApproveAsync(userId, module, currentStep, null))
            throw new UnauthorizedAccessException("User cannot approve this step.");

        transaction.ApprovedBy = userId;
        transaction.ApprovedAt = DateTime.UtcNow;
        transaction.ApprovedStep = currentStep;

        if (step.IsFinal)
        {
            transaction.Status = ApprovalStatus.Approved;
        }
        else
        {
            var nextStep = workflow.Steps.OrderBy(s => s.StepOrder).FirstOrDefault(s => s.StepOrder > currentStep);
            if (nextStep == null)
            {
                transaction.Status = ApprovalStatus.Approved;
            }
            else
            {
                transaction.CurrentStep = nextStep.StepOrder;
            }
        }

        transaction.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return transaction;
    }

    public async Task<ApprovalTransaction> RejectAsync(string userId, string module, int recordId, string reason)
    {
        var transaction = await GetApprovalTransactionAsync(module, recordId);
        if (transaction == null) throw new InvalidOperationException("Approval transaction not found.");
        if (transaction.Status != ApprovalStatus.Pending)
            throw new InvalidOperationException("Approval transaction is not pending.");

        if (!await CanApproveAsync(userId, module, transaction.CurrentStep, null))
            throw new UnauthorizedAccessException("User cannot reject this step.");

        transaction.Status = ApprovalStatus.Rejected;
        transaction.ApprovedBy = userId;
        transaction.ApprovedAt = DateTime.UtcNow;
        transaction.RejectionReason = reason;
        transaction.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return transaction;
    }

    public async Task<List<ApprovalTransaction>> GetPendingApprovalsAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return new List<ApprovalTransaction>();

        var userRoles = await _userManager.GetRolesAsync(user);

        var pending = await _context.ApprovalTransactions
            .Where(t => t.Status == ApprovalStatus.Pending)
            .ToListAsync();

        var workflows = await _context.ApprovalWorkflows.Include(w => w.Steps).Where(w => w.IsActive && !w.IsDeleted).ToListAsync();

        return pending.Where(t =>
        {
            var workflow = workflows.FirstOrDefault(w => w.Module == t.Module);
            if (workflow == null) return false;

            var step = workflow.Steps.FirstOrDefault(s => s.StepOrder == t.CurrentStep);
            if (step == null) return false;

            if (!userRoles.Contains(step.RoleName, StringComparer.OrdinalIgnoreCase))
                return false;

            if (step.IsDepartmentScoped && user.DepartmentId.HasValue)
            {
                // we assume module-specific data is not available here, so we'll include same-department approval only by role
                return true;
            }

            return true;
        }).ToList();
    }
}
using Microsoft.EntityFrameworkCore;
using SuryaPolyFlex.Application.Common.Interfaces;
using SuryaPolyFlex.Domain.Entities.Core;
using SuryaPolyFlex.Infrastructure.Data;

namespace SuryaPolyFlex.Infrastructure.Services;

public class RuleEvaluationService : IRuleEvaluationService
{
    private readonly AppDbContext _context;

    public RuleEvaluationService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> EvaluateRulesAsync(string permissionName, string userId, object resourceContext)
    {
        var rules = await _context.Rules
            .Where(r => r.Permission.Name == permissionName && !r.IsDeleted && r.IsActive)
            .Include(r => r.Permission)
            .ToListAsync();

        if (!rules.Any())
            return true; // No rules configured => allow

        var context = resourceContext as Dictionary<string, object>;
        if (context == null)
            throw new ArgumentException("resourceContext must be a Dictionary<string, object>");

        var userDepartmentId = context.TryGetValue("UserDepartmentId", out var ud) && ud is int udv ? udv : (int?)null;
        var resourceCreatedBy = context.TryGetValue("CreatedBy", out var cb) ? cb?.ToString() : null;
        var resourceDepartmentId = context.TryGetValue("DepartmentId", out var rd) && rd is int rdv ? rdv : (int?)null;

        foreach (var rule in rules)
        {
            if (EvaluateScope(rule.Scope, userId, userDepartmentId, resourceCreatedBy, resourceDepartmentId))
                return true;
        }

        return false;
    }

    private bool EvaluateScope(RuleScope scope, string userId, int? userDepartmentId, string? resourceCreatedBy, int? resourceDepartmentId)
    {
        return scope switch
        {
            RuleScope.All => true,
            RuleScope.Owner => !string.IsNullOrEmpty(resourceCreatedBy) && resourceCreatedBy == userId,
            RuleScope.Department => userDepartmentId != null && resourceDepartmentId != null && userDepartmentId == resourceDepartmentId,
            RuleScope.OwnerOrDepartment => (!string.IsNullOrEmpty(resourceCreatedBy) && resourceCreatedBy == userId) ||
                                           (userDepartmentId != null && resourceDepartmentId != null && userDepartmentId == resourceDepartmentId),
            _ => false
        };
    }
}
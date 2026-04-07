using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SuryaPolyFlex.Domain.Entities.Core;
using SuryaPolyFlex.Domain.Entities.Inventory;
using SuryaPolyFlex.Domain.Enums;
using SuryaPolyFlex.Application.Common;

namespace SuryaPolyFlex.Infrastructure.Data;

public static class SeedData
{
    public static async Task SeedAsync(
        AppDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager)
    {
        await context.Database.MigrateAsync();

        // ── 1. Roles ──────────────────────────────────────────────
        var roles = new[]
        {
            new ApplicationRole { Name = "Admin",          Description = "Full system access",       IsActive = true },
            new ApplicationRole { Name = "StoreManager",   Description = "Procurement and inventory", IsActive = true },
            new ApplicationRole { Name = "SalesManager",   Description = "Sales and CRM",             IsActive = true },
            new ApplicationRole { Name = "Production",     Description = "Production floor access",   IsActive = true },
            new ApplicationRole { Name = "Dispatch",       Description = "Dispatch and logistics",    IsActive = true },
            new ApplicationRole { Name = "Accounts",       Description = "Finance and reports",       IsActive = true },
            new ApplicationRole { Name = "DepartmentHead", Description = "Department level approver", IsActive = true },
            new ApplicationRole { Name = "Manager",        Description = "General manager role",      IsActive = true },
            new ApplicationRole { Name = "Employee",       Description = "General employee role",     IsActive = true }
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role.Name!))
                await roleManager.CreateAsync(role);
        }

        // ── 2.5. Permissions ──────────────────────────────────────
        if (!await context.Permissions.AnyAsync())
        {
            var permissions = GetAllPermissions().Select(p => new Permission
            {
                Name = p,
                Module = p.Split('_')[0],
                Action = p.Split('_')[1],
                Description = $"{p.Split('_')[1]} {p.Split('_')[0]}",
                CreatedBy = "SYSTEM",
                CreatedAt = DateTime.UtcNow
            });
            context.Permissions.AddRange(permissions);
        }

        // ── 3. Default Admin User ─────────────────────────────────
        const string adminEmail    = "admin@suryapolyflex.com";
        const string adminPassword = "Admin@12345";

        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var adminUser = new ApplicationUser
            {
                UserName       = adminEmail,
                Email          = adminEmail,
                FullName       = "System Administrator",
                AuthorityRole  = AuthorityRole.Admin,
                IsActive       = true,
                EmailConfirmed = true,
                CreatedAt      = DateTime.UtcNow,
                CreatedBy      = "SYSTEM"
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
                await userManager.AddToRoleAsync(adminUser, "Admin");
        }

        // ── 3. Number Sequences — dynamic FY ─────────────────────
        var now = DateTime.Now;
        var fy  = now.Month >= 4
            ? $"{now.Year % 100}{(now.Year + 1) % 100}"
            : $"{(now.Year - 1) % 100}{now.Year % 100}";

        var moduleCodes = new[]
        {
            ("INDENT", "IND"),
            ("PO",     "PO"),
            ("GRN",    "GRN"),
            ("SO",     "SO"),
            ("JC",     "JC"),
            ("WO",     "WO"),
            ("DC",     "DC"),
            ("GATE",   "GATE"),
            ("QTN",    "QTN"),
            ("MIS",    "MIS"),
        };

        foreach (var (moduleCode, prefix) in moduleCodes)
        {
            var exists = await context.NumberSequences
                .AnyAsync(n => n.ModuleCode == moduleCode && n.FinancialYear == fy);

            if (!exists)
            {
                context.NumberSequences.Add(new NumberSequence
                {
                    ModuleCode    = moduleCode,
                    Prefix        = prefix,
                    LastNumber    = 0,
                    PaddingLength = 5,
                    FinancialYear = fy,
                    CreatedBy     = "SYSTEM",
                    CreatedAt     = DateTime.UtcNow
                });
            }
        }

        // ── 4. Default Departments ────────────────────────────────
        if (!await context.Departments.AnyAsync())
        {
            context.Departments.AddRange(
                new Department { Code = "ADMIN", Name = "Administration", IsActive = true, CreatedBy = "SYSTEM" },
                new Department { Code = "STORE", Name = "Store",          IsActive = true, CreatedBy = "SYSTEM" },
                new Department { Code = "SALES", Name = "Sales",          IsActive = true, CreatedBy = "SYSTEM" },
                new Department { Code = "PROD",  Name = "Production",     IsActive = true, CreatedBy = "SYSTEM" },
                new Department { Code = "DISP",  Name = "Dispatch",       IsActive = true, CreatedBy = "SYSTEM" }
            );
        }

        // ── 5. Units of Measure ───────────────────────────────────
        if (!await context.UnitOfMeasures.AnyAsync())
        {
            context.UnitOfMeasures.AddRange(
                new UnitOfMeasure { Code = "KG",  Name = "Kilogram", IsActive = true, CreatedBy = "SYSTEM" },
                new UnitOfMeasure { Code = "GRM", Name = "Gram",     IsActive = true, CreatedBy = "SYSTEM" },
                new UnitOfMeasure { Code = "LTR", Name = "Litre",    IsActive = true, CreatedBy = "SYSTEM" },
                new UnitOfMeasure { Code = "MTR", Name = "Metre",    IsActive = true, CreatedBy = "SYSTEM" },
                new UnitOfMeasure { Code = "NOS", Name = "Numbers",  IsActive = true, CreatedBy = "SYSTEM" },
                new UnitOfMeasure { Code = "BOX", Name = "Box",      IsActive = true, CreatedBy = "SYSTEM" },
                new UnitOfMeasure { Code = "ROL", Name = "Roll",     IsActive = true, CreatedBy = "SYSTEM" }
            );
        }

        // ── 6. Item Categories ────────────────────────────────────
        if (!await context.ItemCategories.AnyAsync())
        {
            context.ItemCategories.AddRange(
                new ItemCategory { Code = "INK",  Name = "Inks",           IsActive = true, CreatedBy = "SYSTEM" },
                new ItemCategory { Code = "SUB",  Name = "Substrates",     IsActive = true, CreatedBy = "SYSTEM" },
                new ItemCategory { Code = "CHEM", Name = "Chemicals",      IsActive = true, CreatedBy = "SYSTEM" },
                new ItemCategory { Code = "PKG",  Name = "Packaging",      IsActive = true, CreatedBy = "SYSTEM" },
                new ItemCategory { Code = "CONS", Name = "Consumables",    IsActive = true, CreatedBy = "SYSTEM" },
                new ItemCategory { Code = "FG",   Name = "Finished Goods", IsActive = true, CreatedBy = "SYSTEM" }
            );
        }

        // ── 7. Warehouses ─────────────────────────────────────────
        if (!await context.Warehouses.AnyAsync())
        {
            context.Warehouses.AddRange(
                new Warehouse { Code = "RM-STORE", Name = "Raw Material Store",   WarehouseType = "RM",    IsActive = true, CreatedBy = "SYSTEM" },
                new Warehouse { Code = "FG-STORE", Name = "Finished Goods Store", WarehouseType = "FG",    IsActive = true, CreatedBy = "SYSTEM" },
                new Warehouse { Code = "WIP",      Name = "Work In Progress",     WarehouseType = "WIP",   IsActive = true, CreatedBy = "SYSTEM" },
                new Warehouse { Code = "SCRAP",    Name = "Scrap Yard",           WarehouseType = "Scrap", IsActive = true, CreatedBy = "SYSTEM" }
            );
        }

        // ── 8. Initial Rule + Approval Workflow Seed ──────────────
        await SeedRolePermissionsAsync(context);
        await SeedRulesAsync(context);
        await SeedApprovalWorkflowsAsync(context);
        await SeedUserPermissionOverridesAsync(context);

        await context.SaveChangesAsync();
    }

    private static List<string> GetAllPermissions()
    {
        return typeof(SuryaPolyFlex.Application.Common.Permissions)
            .GetNestedTypes()
            .SelectMany(t => t.GetFields())
            .Select(f => f.GetValue(null)?.ToString() ?? "")
            .Where(v => !string.IsNullOrEmpty(v))
            .ToList();
    }

    private static async Task SeedRolePermissionsAsync(AppDbContext context)
    {
        if (await context.RolePermissions.AnyAsync())
            return;

        var roleMap = new Dictionary<string, string[]>
        {
            ["Admin"] = GetAllPermissions().ToArray(),
            ["Employee"] = new[]
            {
                Permissions.Indents.View, Permissions.Indents.Create, Permissions.Indents.Edit,
                Permissions.PurchaseOrders.View, Permissions.PurchaseOrders.Create, Permissions.PurchaseOrders.Edit,
                Permissions.SalesOrders.View, Permissions.SalesOrders.Create, Permissions.SalesOrders.Edit,
                Permissions.Dispatch.View, Permissions.Dispatch.Create, Permissions.Dispatch.Edit
            },
            ["Manager"] = new[]
            {
                Permissions.Indents.View, Permissions.Indents.Create, Permissions.Indents.Edit, Permissions.Indents.Approve,
                Permissions.PurchaseOrders.View, Permissions.PurchaseOrders.Create, Permissions.PurchaseOrders.Edit, Permissions.PurchaseOrders.Approve
            },
            ["DepartmentHead"] = new[]
            {
                Permissions.Indents.View, Permissions.Indents.Create, Permissions.Indents.Edit, Permissions.Indents.Approve,
                Permissions.PurchaseOrders.View, Permissions.PurchaseOrders.Create, Permissions.PurchaseOrders.Edit, Permissions.PurchaseOrders.Approve
            },
            ["SalesManager"] = new[]
            {
                Permissions.SalesOrders.View, Permissions.SalesOrders.Create, Permissions.SalesOrders.Edit, Permissions.SalesOrders.Approve
            },
            ["StoreManager"] = new[]
            {
                Permissions.PurchaseOrders.View, Permissions.PurchaseOrders.Create, Permissions.PurchaseOrders.Edit, Permissions.PurchaseOrders.Approve,
                Permissions.GRN.View, Permissions.GRN.Create, Permissions.GRN.Edit
            },
            ["Dispatch"] = new[]
            {
                Permissions.Dispatch.View, Permissions.Dispatch.Create, Permissions.Dispatch.Edit, Permissions.Dispatch.Export
            }
        };

        var roleIds = await context.Roles
            .Where(r => roleMap.Keys.Contains(r.Name!))
            .ToDictionaryAsync(r => r.Name!, r => r.Id);

        var permissions = await context.Permissions.ToDictionaryAsync(p => p.Name, p => p.Id);

        foreach (var (role, perms) in roleMap)
        {
            if (!roleIds.TryGetValue(role, out var roleId))
                continue;

            foreach (var perm in perms.Distinct())
            {
                if (!permissions.TryGetValue(perm, out var permissionId))
                    continue;

                context.RolePermissions.Add(new RolePermission
                {
                    RoleId = roleId,
                    PermissionId = permissionId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "SYSTEM"
                });
            }
        }
    }

    private static async Task SeedRulesAsync(AppDbContext context)
    {
        if (await context.Rules.AnyAsync())
            return;

        var permissionCandidates = await context.Permissions.ToDictionaryAsync(p => p.Name, p => p.Id);

        void AddRule(string permissionName, RuleScope scope, string name, string desc)
        {
            if (!permissionCandidates.TryGetValue(permissionName, out var permissionId))
                return;

            context.Rules.Add(new Rule
            {
                Name = name,
                Description = desc,
                PermissionId = permissionId,
                Scope = scope,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "SYSTEM"
            });
        }

        AddRule(Permissions.Indents.View, RuleScope.OwnerOrDepartment, "Indents View", "Employee and manager can view own or department records");
        AddRule(Permissions.Indents.Create, RuleScope.All, "Indents Create", "Any eligible role can create indents");
        AddRule(Permissions.Indents.Edit, RuleScope.Owner, "Indents Edit", "Only owner can edit indents");
        AddRule(Permissions.Indents.Approve, RuleScope.Department, "Indents Approve", "Department head can approve indents");

        AddRule(Permissions.PurchaseOrders.View, RuleScope.Department, "PO View", "Manager and above can view department PO");
        AddRule(Permissions.PurchaseOrders.Create, RuleScope.All, "PO Create", "Eligible role can create POs");
        AddRule(Permissions.PurchaseOrders.Edit, RuleScope.Owner, "PO Edit", "Owner PO can edit");
        AddRule(Permissions.PurchaseOrders.Approve, RuleScope.Department, "PO Approve", "Manager approves PO");

        AddRule(Permissions.SalesOrders.View, RuleScope.Department, "SO View", "Sales users may view department sales orders");
        AddRule(Permissions.SalesOrders.Create, RuleScope.All, "SO Create", "Sales order creation");
        AddRule(Permissions.SalesOrders.Edit, RuleScope.Owner, "SO Edit", "Owner edits");
        AddRule(Permissions.SalesOrders.Approve, RuleScope.Department, "SO Approve", "Sales manager approval");

        AddRule(Permissions.Dispatch.View, RuleScope.Department, "Dispatch View", "Dispatch team can view department dispatches");
        AddRule(Permissions.Dispatch.Create, RuleScope.All, "Dispatch Create", "Dispatch creation");
        AddRule(Permissions.Dispatch.Edit, RuleScope.Owner, "Dispatch Edit", "Owner edits");
    }

    private static async Task SeedApprovalWorkflowsAsync(AppDbContext context)
    {
        if (await context.ApprovalWorkflows.AnyAsync())
            return;

        var indentsWorkflow = new ApprovalWorkflow
        {
            Module = "INDENTS",
            Name = "Indents Approval Flow",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "SYSTEM",
            Steps = new List<ApprovalStep>
            {
                new ApprovalStep { StepOrder = 1, RoleName = "Employee", IsDepartmentScoped = true, IsFinal = false, CreatedAt = DateTime.UtcNow, CreatedBy = "SYSTEM" },
                new ApprovalStep { StepOrder = 2, RoleName = "DepartmentHead", IsDepartmentScoped = true, IsFinal = false, CreatedAt = DateTime.UtcNow, CreatedBy = "SYSTEM" },
                new ApprovalStep { StepOrder = 3, RoleName = "Admin", IsDepartmentScoped = false, IsFinal = true, CreatedAt = DateTime.UtcNow, CreatedBy = "SYSTEM" }
            }
        };

        var poWorkflow = new ApprovalWorkflow
        {
            Module = "PO",
            Name = "Purchase Order Approval Flow",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "SYSTEM",
            Steps = new List<ApprovalStep>
            {
                new ApprovalStep { StepOrder = 1, RoleName = "Manager", IsDepartmentScoped = true, IsFinal = false, CreatedAt = DateTime.UtcNow, CreatedBy = "SYSTEM" },
                new ApprovalStep { StepOrder = 2, RoleName = "Admin", IsDepartmentScoped = false, IsFinal = true, CreatedAt = DateTime.UtcNow, CreatedBy = "SYSTEM" }
            }
        };

        var salesWorkflow = new ApprovalWorkflow
        {
            Module = "SO",
            Name = "Sales Order Approval Flow",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "SYSTEM",
            Steps = new List<ApprovalStep>
            {
                new ApprovalStep { StepOrder = 1, RoleName = "SalesManager", IsDepartmentScoped = true, IsFinal = false, CreatedAt = DateTime.UtcNow, CreatedBy = "SYSTEM" },
                new ApprovalStep { StepOrder = 2, RoleName = "Admin", IsDepartmentScoped = false, IsFinal = true, CreatedAt = DateTime.UtcNow, CreatedBy = "SYSTEM" }
            }
        };

        var dispatchWorkflow = new ApprovalWorkflow
        {
            Module = "DISPATCH",
            Name = "Dispatch Approval Flow",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "SYSTEM",
            Steps = new List<ApprovalStep>
            {
                new ApprovalStep { StepOrder = 1, RoleName = "Employee", IsDepartmentScoped = true, IsFinal = false, CreatedAt = DateTime.UtcNow, CreatedBy = "SYSTEM" },
                new ApprovalStep { StepOrder = 2, RoleName = "Dispatch", IsDepartmentScoped = true, IsFinal = false, CreatedAt = DateTime.UtcNow, CreatedBy = "SYSTEM" },
                new ApprovalStep { StepOrder = 3, RoleName = "Admin", IsDepartmentScoped = false, IsFinal = true, CreatedAt = DateTime.UtcNow, CreatedBy = "SYSTEM" }
            }
        };

        context.ApprovalWorkflows.AddRange(indentsWorkflow, poWorkflow, salesWorkflow, dispatchWorkflow);
    }

    private static async Task SeedUserPermissionOverridesAsync(AppDbContext context)
    {
        // Only seed once
        if (await context.UserPermissions.AnyAsync())
            return;

        // Get admin user  (test: can add sample user overrides)
        var adminUser = await context.Users.FirstOrDefaultAsync(u =>
            u.Email == "admin@suryapolyflex.com");

        if (adminUser == null)
            return;

        // Example: Admin is explicitly granted these permissions with override status
        // (These serve as examples for how overrides work)
        var permissionsToOverride = new[]
        {
            Permissions.Indents.Approve,
            Permissions.PurchaseOrders.Approve
        };

        var permissions = await context.Permissions
            .Where(p => permissionsToOverride.Contains(p.Name))
            .ToListAsync();

        foreach (var permission in permissions)
        {
            // Check if override already exists
            var existing = await context.UserPermissions
                .FirstOrDefaultAsync(up => up.UserId == adminUser.Id && up.PermissionId == permission.Id);

            if (existing == null)
            {
                context.UserPermissions.Add(new UserPermission
                {
                    UserId = adminUser.Id,
                    PermissionId = permission.Id,
                    IsAllowed = true,  // Admin is allowed everything
                    IsActive = true,
                    CreatedBy = "SYSTEM",
                    CreatedAt = DateTime.UtcNow,
                    ExpiryDate = null  // No expiry for admin overrides
                });
            }
        }
    }
}

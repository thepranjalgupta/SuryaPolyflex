using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SuryaPolyFlex.Domain.Entities.Core;
using SuryaPolyFlex.Domain.Entities.Inventory;

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
            new ApplicationRole { Name = "Admin",        Description = "Full system access",       IsActive = true },
            new ApplicationRole { Name = "StoreManager", Description = "Procurement and inventory", IsActive = true },
            new ApplicationRole { Name = "SalesManager", Description = "Sales and CRM",             IsActive = true },
            new ApplicationRole { Name = "Production",   Description = "Production floor access",   IsActive = true },
            new ApplicationRole { Name = "Dispatch",     Description = "Dispatch and logistics",    IsActive = true },
            new ApplicationRole { Name = "Accounts",     Description = "Finance and reports",       IsActive = true },
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role.Name!))
                await roleManager.CreateAsync(role);
        }

        // ── 2. Default Admin User ─────────────────────────────────
        const string adminEmail    = "admin@suryapolyflex.com";
        const string adminPassword = "Admin@12345";

        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var adminUser = new ApplicationUser
            {
                UserName       = adminEmail,
                Email          = adminEmail,
                FullName       = "System Administrator",
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
            ("MIS", "MIS"),
            ("WO", "WO"),
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

        await context.SaveChangesAsync();
    }
}
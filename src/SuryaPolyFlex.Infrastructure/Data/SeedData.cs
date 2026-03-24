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
        // Run any pending migrations automatically
        await context.Database.MigrateAsync();

        // ── 1. Roles ──────────────────────────────────────────────
        var roles = new[]
        {
            new ApplicationRole { Name = "Admin",       Description = "Full system access",        IsActive = true },
            new ApplicationRole { Name = "StoreManager",Description = "Procurement and inventory",  IsActive = true },
            new ApplicationRole { Name = "SalesManager", Description = "Sales and CRM",             IsActive = true },
            new ApplicationRole { Name = "Production",  Description = "Production floor access",    IsActive = true },
            new ApplicationRole { Name = "Dispatch",    Description = "Dispatch and logistics",     IsActive = true },
            new ApplicationRole { Name = "Accounts",    Description = "Finance and reports",        IsActive = true },
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
                UserName   = adminEmail,
                Email      = adminEmail,
                FullName   = "System Administrator",
                IsActive   = true,
                EmailConfirmed = true,
                CreatedAt  = DateTime.UtcNow,
                CreatedBy  = "SYSTEM"
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
                await userManager.AddToRoleAsync(adminUser, "Admin");
        }

        // ── 3. Number Sequences ───────────────────────────────────
        var sequences = new[]
        {
            new NumberSequence { ModuleCode = "INDENT",  Prefix = "IND",  LastNumber = 0, PaddingLength = 5, FinancialYear = "2425", CreatedBy = "SYSTEM" },
            new NumberSequence { ModuleCode = "PO",      Prefix = "PO",   LastNumber = 0, PaddingLength = 5, FinancialYear = "2425", CreatedBy = "SYSTEM" },
            new NumberSequence { ModuleCode = "GRN",     Prefix = "GRN",  LastNumber = 0, PaddingLength = 5, FinancialYear = "2425", CreatedBy = "SYSTEM" },
            new NumberSequence { ModuleCode = "SO",      Prefix = "SO",   LastNumber = 0, PaddingLength = 5, FinancialYear = "2425", CreatedBy = "SYSTEM" },
            new NumberSequence { ModuleCode = "JC",      Prefix = "JC",   LastNumber = 0, PaddingLength = 5, FinancialYear = "2425", CreatedBy = "SYSTEM" },
            new NumberSequence { ModuleCode = "WO",      Prefix = "WO",   LastNumber = 0, PaddingLength = 5, FinancialYear = "2425", CreatedBy = "SYSTEM" },
            new NumberSequence { ModuleCode = "DC",      Prefix = "DC",   LastNumber = 0, PaddingLength = 5, FinancialYear = "2425", CreatedBy = "SYSTEM" },
            new NumberSequence { ModuleCode = "GATE",    Prefix = "GATE", LastNumber = 0, PaddingLength = 5, FinancialYear = "2425", CreatedBy = "SYSTEM" },
            new NumberSequence { ModuleCode = "QTN",     Prefix = "QTN",  LastNumber = 0, PaddingLength = 5, FinancialYear = "2425", CreatedBy = "SYSTEM" },
        };

        foreach (var seq in sequences)
        {
            var exists = await context.NumberSequences
                .AnyAsync(n => n.ModuleCode == seq.ModuleCode && n.FinancialYear == seq.FinancialYear);
            if (!exists)
                context.NumberSequences.Add(seq);
        }

        // ── 4. Default Department ─────────────────────────────────
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
        new UnitOfMeasure { Code = "KG",  Name = "Kilogram",  IsActive = true, CreatedBy = "SYSTEM" },
        new UnitOfMeasure { Code = "GRM", Name = "Gram",      IsActive = true, CreatedBy = "SYSTEM" },
        new UnitOfMeasure { Code = "LTR", Name = "Litre",     IsActive = true, CreatedBy = "SYSTEM" },
        new UnitOfMeasure { Code = "MTR", Name = "Metre",     IsActive = true, CreatedBy = "SYSTEM" },
        new UnitOfMeasure { Code = "NOS", Name = "Numbers",   IsActive = true, CreatedBy = "SYSTEM" },
        new UnitOfMeasure { Code = "BOX", Name = "Box",       IsActive = true, CreatedBy = "SYSTEM" },
        new UnitOfMeasure { Code = "ROL", Name = "Roll",      IsActive = true, CreatedBy = "SYSTEM" }
    );
}

// ── 6. Item Categories ────────────────────────────────────
if (!await context.ItemCategories.AnyAsync())
{
    context.ItemCategories.AddRange(
        new ItemCategory { Code = "INK",  Name = "Inks",             IsActive = true, CreatedBy = "SYSTEM" },
        new ItemCategory { Code = "SUB",  Name = "Substrates",       IsActive = true, CreatedBy = "SYSTEM" },
        new ItemCategory { Code = "CHEM", Name = "Chemicals",        IsActive = true, CreatedBy = "SYSTEM" },
        new ItemCategory { Code = "PKG",  Name = "Packaging",        IsActive = true, CreatedBy = "SYSTEM" },
        new ItemCategory { Code = "CONS", Name = "Consumables",      IsActive = true, CreatedBy = "SYSTEM" },
        new ItemCategory { Code = "FG",   Name = "Finished Goods",   IsActive = true, CreatedBy = "SYSTEM" }
    );
}

// ── 7. Warehouses ─────────────────────────────────────────
if (!await context.Warehouses.AnyAsync())
{
    context.Warehouses.AddRange(
        new Warehouse { Code = "RM-STORE", Name = "Raw Material Store",  WarehouseType = "RM",    IsActive = true, CreatedBy = "SYSTEM" },
        new Warehouse { Code = "FG-STORE", Name = "Finished Goods Store", WarehouseType = "FG",   IsActive = true, CreatedBy = "SYSTEM" },
        new Warehouse { Code = "WIP",      Name = "Work In Progress",     WarehouseType = "WIP",  IsActive = true, CreatedBy = "SYSTEM" },
        new Warehouse { Code = "SCRAP",    Name = "Scrap Yard",           WarehouseType = "Scrap", IsActive = true, CreatedBy = "SYSTEM" }
    );
}

        await context.SaveChangesAsync();
    }
}
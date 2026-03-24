using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SuryaPolyFlex.Domain.Entities.Core;
using SuryaPolyFlex.Domain.Entities.Procurement;
using SuryaPolyFlex.Domain.Entities.Sales;
using SuryaPolyFlex.Domain.Entities.Inventory;

namespace SuryaPolyFlex.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Core
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<NumberSequence> NumberSequences => Set<NumberSequence>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<WorkflowAction> WorkflowActions => Set<WorkflowAction>();
    public DbSet<Attachment> Attachments => Set<Attachment>();
    public DbSet<RoleModulePermission> RoleModulePermissions => Set<RoleModulePermission>();
    // Procurement masters
public DbSet<Vendor> Vendors => Set<Vendor>();

// Sales masters
public DbSet<Customer> Customers => Set<Customer>();
// Inventory
public DbSet<UnitOfMeasure> UnitOfMeasures  => Set<UnitOfMeasure>();
public DbSet<ItemCategory>  ItemCategories  => Set<ItemCategory>();
public DbSet<Item>          Items           => Set<Item>();
public DbSet<Warehouse>     Warehouses      => Set<Warehouse>();
public DbSet<StockBalance>  StockBalances   => Set<StockBalance>();
public DbSet<StockLedger>   StockLedgers    => Set<StockLedger>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Schema assignments
        builder.Entity<Department>().ToTable("Departments", "dbo");
        builder.Entity<Employee>().ToTable("Employees", "dbo");
        builder.Entity<NumberSequence>().ToTable("NumberSequences", "dbo");
        builder.Entity<AuditLog>().ToTable("AuditLogs", "security");
        builder.Entity<Notification>().ToTable("Notifications", "dbo");
        builder.Entity<WorkflowAction>().ToTable("WorkflowActions", "dbo");
        builder.Entity<Attachment>().ToTable("Attachments", "dbo");
        builder.Entity<RoleModulePermission>().ToTable("RoleModulePermissions", "security");

        // Identity tables into security schema
        builder.Entity<ApplicationUser>().ToTable("Users", "security");
        builder.Entity<ApplicationRole>().ToTable("Roles", "security");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserRole<string>>().ToTable("UserRoles", "security");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserClaim<string>>().ToTable("UserClaims", "security");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserLogin<string>>().ToTable("UserLogins", "security");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>>().ToTable("RoleClaims", "security");
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityUserToken<string>>().ToTable("UserTokens", "security");
// Vendor
builder.Entity<Vendor>().ToTable("Vendors", "proc");
builder.Entity<Vendor>().HasIndex(v => v.VendorCode).IsUnique();
builder.Entity<Vendor>().HasQueryFilter(v => !v.IsDeleted);

// Customer
builder.Entity<Customer>().ToTable("Customers", "sales");
builder.Entity<Customer>().HasIndex(c => c.CustomerCode).IsUnique();
builder.Entity<Customer>().HasQueryFilter(c => !c.IsDeleted);
builder.Entity<Customer>().Property(c => c.CreditLimit).HasPrecision(18, 2);
        

        // Unique indexes
        builder.Entity<Department>()
            .HasIndex(d => d.Code).IsUnique();

        builder.Entity<Employee>()
            .HasIndex(e => e.EmployeeCode).IsUnique();

        builder.Entity<NumberSequence>()
            .HasIndex(n => new { n.ModuleCode, n.FinancialYear }).IsUnique();

        // Soft delete global query filters
        builder.Entity<Department>().HasQueryFilter(d => !d.IsDeleted);
        builder.Entity<Employee>().HasQueryFilter(e => !e.IsDeleted);

        // Decimal precision
        // Add more here as financial tables come in

        // Relationships
        builder.Entity<Employee>()
            .HasOne(e => e.Department)
            .WithMany(d => d.Employees)
            .HasForeignKey(e => e.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<RoleModulePermission>()
            .HasOne(r => r.Role)
            .WithMany()
            .HasForeignKey(r => r.RoleId)
            .OnDelete(DeleteBehavior.Cascade);





            // Inventory schema
builder.Entity<UnitOfMeasure>().ToTable("UnitOfMeasures", "inv");
builder.Entity<UnitOfMeasure>().HasIndex(u => u.Code).IsUnique();
builder.Entity<UnitOfMeasure>().HasQueryFilter(u => !u.IsDeleted);

builder.Entity<ItemCategory>().ToTable("ItemCategories", "inv");
builder.Entity<ItemCategory>().HasIndex(c => c.Code).IsUnique();
builder.Entity<ItemCategory>().HasQueryFilter(c => !c.IsDeleted);

builder.Entity<Item>().ToTable("Items", "inv");
builder.Entity<Item>().HasIndex(i => i.ItemCode).IsUnique();
builder.Entity<Item>().HasQueryFilter(i => !i.IsDeleted);
builder.Entity<Item>().Property(i => i.MinStockLevel).HasPrecision(18, 3);
builder.Entity<Item>().Property(i => i.ReorderQty).HasPrecision(18, 3);
builder.Entity<Item>().Property(i => i.StandardCost).HasPrecision(18, 2);
builder.Entity<Item>()
    .HasOne(i => i.Category)
    .WithMany(c => c.Items)
    .HasForeignKey(i => i.CategoryId)
    .OnDelete(DeleteBehavior.Restrict);
builder.Entity<Item>()
    .HasOne(i => i.UoM)
    .WithMany()
    .HasForeignKey(i => i.UoMId)
    .OnDelete(DeleteBehavior.Restrict);

builder.Entity<Warehouse>().ToTable("Warehouses", "inv");
builder.Entity<Warehouse>().HasIndex(w => w.Code).IsUnique();
builder.Entity<Warehouse>().HasQueryFilter(w => !w.IsDeleted);

builder.Entity<StockBalance>().ToTable("StockBalances", "inv");
builder.Entity<StockBalance>().HasIndex(s => new { s.ItemId, s.WarehouseId }).IsUnique();
builder.Entity<StockBalance>().Property(s => s.OnHandQty).HasPrecision(18, 3);
builder.Entity<StockBalance>().Property(s => s.ReservedQty).HasPrecision(18, 3);
builder.Entity<StockBalance>().Ignore(s => s.AvailableQty);
builder.Entity<StockBalance>().HasQueryFilter(s =>
    !s.Item.IsDeleted && !s.Warehouse.IsDeleted);



builder.Entity<StockLedger>().ToTable("StockLedgers", "inv");
builder.Entity<StockLedger>().Property(s => s.InwardQty).HasPrecision(18, 3);
builder.Entity<StockLedger>().Property(s => s.OutwardQty).HasPrecision(18, 3);
builder.Entity<StockLedger>().Property(s => s.BalanceQty).HasPrecision(18, 3);
builder.Entity<StockLedger>().Property(s => s.UnitCost).HasPrecision(18, 2);
builder.Entity<StockLedger>().HasQueryFilter(s =>
    !s.Item.IsDeleted && !s.Warehouse.IsDeleted);
    }
}
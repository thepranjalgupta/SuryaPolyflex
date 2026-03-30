using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SuryaPolyFlex.Domain.Entities.Core;
using SuryaPolyFlex.Domain.Entities.Procurement;
using SuryaPolyFlex.Domain.Entities.Sales;
using SuryaPolyFlex.Domain.Entities.Inventory;
using SuryaPolyFlex.Domain.Entities.Production;
using SuryaPolyFlex.Domain.Entities.Dispatch;



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


// Procurement
public DbSet<Indent>        Indents        => Set<Indent>();
public DbSet<IndentItem>    IndentItems    => Set<IndentItem>();
public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
public DbSet<POItem>        POItems        => Set<POItem>();
public DbSet<GRN>           GRNs           => Set<GRN>();
public DbSet<GRNItem>       GRNItems       => Set<GRNItem>();

// Sales
public DbSet<SuryaPolyFlex.Domain.Entities.Sales.Lead> Leads => Set<SuryaPolyFlex.Domain.Entities.Sales.Lead>();
public DbSet<SuryaPolyFlex.Domain.Entities.Sales.Quotation> Quotations => Set<SuryaPolyFlex.Domain.Entities.Sales.Quotation>();
public DbSet<SuryaPolyFlex.Domain.Entities.Sales.QuotationItem> QuotationItems => Set<SuryaPolyFlex.Domain.Entities.Sales.QuotationItem>();
public DbSet<SuryaPolyFlex.Domain.Entities.Sales.SalesOrder> SalesOrders => Set<SuryaPolyFlex.Domain.Entities.Sales.SalesOrder>();
public DbSet<SuryaPolyFlex.Domain.Entities.Sales.SOItem> SOItems => Set<SuryaPolyFlex.Domain.Entities.Sales.SOItem>();
public DbSet<SuryaPolyFlex.Domain.Entities.Sales.CustomerJob> CustomerJobs => Set<SuryaPolyFlex.Domain.Entities.Sales.CustomerJob>();

// Production
public DbSet<Machine>         Machines         => Set<Machine>();
public DbSet<JobCard>         JobCards         => Set<JobCard>();
public DbSet<BOM>             BOMs             => Set<BOM>();
public DbSet<BOMItem>         BOMItems         => Set<BOMItem>();
public DbSet<WorkOrder>       WorkOrders       => Set<WorkOrder>();
public DbSet<ProductionEntry> ProductionEntries => Set<ProductionEntry>();
public DbSet<FloorStock>      FloorStocks      => Set<FloorStock>();
public DbSet<InkReturn>       InkReturns       => Set<InkReturn>();
public DbSet<InkReturnItem>   InkReturnItems   => Set<InkReturnItem>();
public DbSet<Scrap>           Scraps           => Set<Scrap>();

// Material Issue
public DbSet<MaterialIssue>     MaterialIssues     => Set<MaterialIssue>();
public DbSet<MaterialIssueItem> MaterialIssueItems => Set<MaterialIssueItem>();

// Dispatch
public DbSet<Transporter>     Transporters     => Set<Transporter>();
public DbSet<DispatchPlan>    DispatchPlans    => Set<DispatchPlan>();
public DbSet<DispatchPlanItem> DispatchPlanItems => Set<DispatchPlanItem>();
public DbSet<DeliveryChallan> DeliveryChallans => Set<DeliveryChallan>();
public DbSet<ChallanItem>     ChallanItems     => Set<ChallanItem>();
public DbSet<Shipment>        Shipments        => Set<Shipment>();


// Sales Returns
public DbSet<SalesReturn>     SalesReturns     => Set<SalesReturn>();
public DbSet<SalesReturnItem> SalesReturnItems => Set<SalesReturnItem>();

// Gate Entry
public DbSet<GateEntry> GateEntries => Set<GateEntry>();

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





    // Procurement schema
builder.Entity<Indent>().ToTable("Indents", "proc");
builder.Entity<Indent>().HasIndex(i => i.IndentNumber).IsUnique();
builder.Entity<Indent>().HasQueryFilter(i =>
    i.Department == null || !i.Department.IsDeleted);


builder.Entity<IndentItem>().ToTable("IndentItems", "proc");
builder.Entity<IndentItem>().HasQueryFilter(i =>
    i.Item == null || !i.Item.IsDeleted);
builder.Entity<IndentItem>().Property(i => i.RequestedQty).HasPrecision(18, 3);
builder.Entity<IndentItem>().Property(i => i.ApprovedQty).HasPrecision(18, 3);

builder.Entity<PurchaseOrder>().ToTable("PurchaseOrders", "proc");
builder.Entity<PurchaseOrder>().HasIndex(p => p.PONumber).IsUnique();
builder.Entity<PurchaseOrder>().Property(p => p.TotalAmount).HasPrecision(18, 2);
builder.Entity<PurchaseOrder>().HasQueryFilter(p =>
    !p.Vendor.IsDeleted);

builder.Entity<POItem>().ToTable("POItems", "proc");
builder.Entity<POItem>().Ignore(p => p.PendingQty);
builder.Entity<POItem>().Ignore(p => p.LineTotal);
builder.Entity<POItem>().Property(p => p.OrderedQty).HasPrecision(18, 3);
builder.Entity<POItem>().Property(p => p.ReceivedQty).HasPrecision(18, 3);
builder.Entity<POItem>().Property(p => p.UnitPrice).HasPrecision(18, 2);
builder.Entity<POItem>().Property(p => p.TaxPct).HasPrecision(5, 2);
builder.Entity<POItem>().HasQueryFilter(p =>
    p.Item == null || !p.Item.IsDeleted);

builder.Entity<GRN>().ToTable("GRNs", "proc");
builder.Entity<GRN>().HasIndex(g => g.GRNNumber).IsUnique();
builder.Entity<GRN>().HasQueryFilter(g =>
    !g.PurchaseOrder.Vendor.IsDeleted);

builder.Entity<GRNItem>().ToTable("GRNItems", "proc");
builder.Entity<GRNItem>().Property(g => g.ReceivedQty).HasPrecision(18, 3);
builder.Entity<GRNItem>().Property(g => g.AcceptedQty).HasPrecision(18, 3);
builder.Entity<GRNItem>().Property(g => g.RejectedQty).HasPrecision(18, 3);
builder.Entity<GRNItem>().Property(g => g.UnitCost).HasPrecision(18, 2);
builder.Entity<GRNItem>().HasQueryFilter(g =>
    g.Item == null || !g.Item.IsDeleted);



    // Sales schema
builder.Entity<Lead>().ToTable("Leads", "sales");
builder.Entity<Lead>()
    .HasOne(l => l.Customer)
    .WithMany()
    .HasForeignKey(l => l.CustomerId)
    .OnDelete(DeleteBehavior.Restrict);
builder.Entity<Lead>().HasQueryFilter(l => !l.IsDeleted);

builder.Entity<Quotation>().ToTable("Quotations", "sales");
builder.Entity<Quotation>().HasIndex(q => q.QuotationNumber).IsUnique();
builder.Entity<Quotation>().Property(q => q.TotalAmount).HasPrecision(18, 2);
builder.Entity<Quotation>()
    .HasOne(q => q.Customer)
    .WithMany()
    .HasForeignKey(q => q.CustomerId)
    .OnDelete(DeleteBehavior.Restrict);
builder.Entity<Quotation>().HasQueryFilter(q => !q.IsDeleted);

builder.Entity<QuotationItem>().ToTable("QuotationItems", "sales");
builder.Entity<QuotationItem>().Ignore(q => q.LineTotal);
builder.Entity<QuotationItem>().Property(q => q.Qty).HasPrecision(18, 3);
builder.Entity<QuotationItem>().Property(q => q.UnitRate).HasPrecision(18, 2);
builder.Entity<QuotationItem>().Property(q => q.DiscountPct).HasPrecision(5, 2);
builder.Entity<QuotationItem>().Property(q => q.TaxPct).HasPrecision(5, 2);
builder.Entity<QuotationItem>()
    .HasOne(q => q.Quotation)
    .WithMany(q => q.Items)
    .HasForeignKey(q => q.QuotationId)
    .OnDelete(DeleteBehavior.Cascade);

builder.Entity<SalesOrder>().ToTable("SalesOrders", "sales");
builder.Entity<SalesOrder>().HasIndex(s => s.SONumber).IsUnique();
builder.Entity<SalesOrder>().Property(s => s.TotalAmount).HasPrecision(18, 2);
builder.Entity<SalesOrder>()
    .HasOne(s => s.Customer)
    .WithMany()
    .HasForeignKey(s => s.CustomerId)
    .OnDelete(DeleteBehavior.Restrict);
builder.Entity<SalesOrder>().HasQueryFilter(s => !s.IsDeleted);

builder.Entity<SOItem>().ToTable("SOItems", "sales");
builder.Entity<SOItem>().Ignore(s => s.PendingQty);
builder.Entity<SOItem>().Ignore(s => s.LineTotal);
builder.Entity<SOItem>().Property(s => s.OrderedQty).HasPrecision(18, 3);
builder.Entity<SOItem>().Property(s => s.DispatchedQty).HasPrecision(18, 3);
builder.Entity<SOItem>().Property(s => s.UnitRate).HasPrecision(18, 2);
builder.Entity<SOItem>().Property(s => s.TaxPct).HasPrecision(5, 2);
builder.Entity<SOItem>()
    .HasOne(s => s.SalesOrder)
    .WithMany(s => s.Items)
    .HasForeignKey(s => s.SalesOrderId)
    .OnDelete(DeleteBehavior.Cascade);

builder.Entity<CustomerJob>().ToTable("CustomerJobs", "sales");
builder.Entity<CustomerJob>().Property(c => c.Width).HasPrecision(10, 2);
builder.Entity<CustomerJob>().Property(c => c.Length).HasPrecision(10, 2);
builder.Entity<CustomerJob>().Property(c => c.Quantity).HasPrecision(18, 3);
builder.Entity<CustomerJob>()
    .HasOne(c => c.SalesOrder)
    .WithMany(s => s.CustomerJobs)
    .HasForeignKey(c => c.SalesOrderId)
    .OnDelete(DeleteBehavior.Cascade);
builder.Entity<CustomerJob>().HasQueryFilter(c => !c.IsDeleted);


// Production schema
builder.Entity<Machine>().ToTable("Machines", "prod");
builder.Entity<Machine>().HasIndex(m => m.MachineCode).IsUnique();
builder.Entity<Machine>().HasQueryFilter(m => !m.IsDeleted);

builder.Entity<JobCard>().ToTable("JobCards", "prod");
builder.Entity<JobCard>().HasIndex(j => j.JobCardNo).IsUnique();
builder.Entity<JobCard>().HasQueryFilter(j => !j.IsDeleted);
builder.Entity<JobCard>().Property(j => j.TargetQty).HasPrecision(18, 3);

builder.Entity<BOM>().ToTable("BOMs", "prod");
builder.Entity<BOM>().HasIndex(b => b.BOMNo).IsUnique();
builder.Entity<BOM>()
    .HasOne(b => b.JobCard)
    .WithMany(j => j.BOMs)
    .HasForeignKey(b => b.JobCardId)
    .OnDelete(DeleteBehavior.Restrict);

builder.Entity<BOMItem>().ToTable("BOMItems", "prod");
builder.Entity<BOMItem>().Property(b => b.RequiredQty).HasPrecision(18, 3);
builder.Entity<BOMItem>().Property(b => b.IssuedQty).HasPrecision(18, 3);
builder.Entity<BOMItem>()
    .HasOne(b => b.BOM)
    .WithMany(b => b.Items)
    .HasForeignKey(b => b.BOMId)
    .OnDelete(DeleteBehavior.Cascade);

builder.Entity<WorkOrder>().ToTable("WorkOrders", "prod");
builder.Entity<WorkOrder>().HasIndex(w => w.WONumber).IsUnique();
builder.Entity<WorkOrder>()
    .HasOne(w => w.JobCard)
    .WithMany(j => j.WorkOrders)
    .HasForeignKey(w => w.JobCardId)
    .OnDelete(DeleteBehavior.Restrict);

builder.Entity<ProductionEntry>().ToTable("ProductionEntries", "prod");
builder.Entity<ProductionEntry>().Property(p => p.ProducedQty).HasPrecision(18, 3);
builder.Entity<ProductionEntry>().Property(p => p.WastageQty).HasPrecision(18, 3);
builder.Entity<ProductionEntry>()
    .HasOne(p => p.WorkOrder)
    .WithMany(w => w.ProductionEntries)
    .HasForeignKey(p => p.WorkOrderId)
    .OnDelete(DeleteBehavior.Restrict);

builder.Entity<FloorStock>().ToTable("FloorStocks", "prod");
builder.Entity<FloorStock>().HasIndex(f => new { f.JobCardId, f.ItemId }).IsUnique();
builder.Entity<FloorStock>().Property(f => f.IssuedQty).HasPrecision(18, 3);
builder.Entity<FloorStock>().Property(f => f.ConsumedQty).HasPrecision(18, 3);
builder.Entity<FloorStock>().Property(f => f.ReturnedQty).HasPrecision(18, 3);
builder.Entity<FloorStock>().Ignore(f => f.BalanceQty);

builder.Entity<InkReturn>().ToTable("InkReturns", "prod");
builder.Entity<InkReturn>().HasIndex(i => i.ReturnNo).IsUnique();
builder.Entity<InkReturn>()
    .HasOne(i => i.JobCard)
    .WithMany()
    .HasForeignKey(i => i.JobCardId)
    .OnDelete(DeleteBehavior.Restrict);

builder.Entity<InkReturnItem>().ToTable("InkReturnItems", "prod");
builder.Entity<InkReturnItem>().Property(i => i.ReturnQty).HasPrecision(18, 3);
builder.Entity<InkReturnItem>()
    .HasOne(i => i.InkReturn)
    .WithMany(i => i.Items)
    .HasForeignKey(i => i.InkReturnId)
    .OnDelete(DeleteBehavior.Cascade);

builder.Entity<Scrap>().ToTable("Scraps", "prod");
builder.Entity<Scrap>().HasIndex(s => s.ScrapNo).IsUnique();
builder.Entity<Scrap>().Property(s => s.ScrapQty).HasPrecision(18, 3);
builder.Entity<Scrap>().Property(s => s.DisposalValue).HasPrecision(18, 2);

// Material Issue
builder.Entity<MaterialIssue>().ToTable("MaterialIssues", "inv");
builder.Entity<MaterialIssue>().HasIndex(m => m.IssueNo).IsUnique();

builder.Entity<MaterialIssueItem>().ToTable("MaterialIssueItems", "inv");
builder.Entity<MaterialIssueItem>().Property(m => m.RequestedQty).HasPrecision(18, 3);
builder.Entity<MaterialIssueItem>().Property(m => m.IssuedQty).HasPrecision(18, 3);
builder.Entity<MaterialIssueItem>().Property(m => m.UnitRate).HasPrecision(18, 2);
builder.Entity<MaterialIssueItem>()
    .HasOne(m => m.MaterialIssue)
    .WithMany(m => m.Items)
    .HasForeignKey(m => m.MaterialIssueId)
    .OnDelete(DeleteBehavior.Cascade);

    // Dispatch schema
builder.Entity<Transporter>().ToTable("Transporters", "dispatch");
builder.Entity<Transporter>().HasQueryFilter(t => !t.IsDeleted);

builder.Entity<DispatchPlan>().ToTable("DispatchPlans", "dispatch");
builder.Entity<DispatchPlan>().HasIndex(d => d.PlanNo).IsUnique();
builder.Entity<DispatchPlan>().HasQueryFilter(d => !d.IsDeleted);

builder.Entity<DispatchPlanItem>().ToTable("DispatchPlanItems", "dispatch");
builder.Entity<DispatchPlanItem>().Property(d => d.DispatchQty).HasPrecision(18, 3);
builder.Entity<DispatchPlanItem>()
    .HasOne(d => d.DispatchPlan)
    .WithMany(d => d.Items)
    .HasForeignKey(d => d.DispatchPlanId)
    .OnDelete(DeleteBehavior.Cascade);
builder.Entity<DispatchPlanItem>()
    .HasOne(d => d.SalesOrder)
    .WithMany()
    .HasForeignKey(d => d.SOId)
    .OnDelete(DeleteBehavior.Restrict);

builder.Entity<DeliveryChallan>().ToTable("DeliveryChallans", "dispatch");
builder.Entity<DeliveryChallan>().HasIndex(d => d.ChallanNo).IsUnique();
builder.Entity<DeliveryChallan>().Property(d => d.TotalValue).HasPrecision(18, 2);
builder.Entity<DeliveryChallan>().HasQueryFilter(d => !d.IsDeleted);
builder.Entity<DeliveryChallan>()
    .HasOne(d => d.SalesOrder)
    .WithMany()
    .HasForeignKey(d => d.SOId)
    .OnDelete(DeleteBehavior.Restrict);
builder.Entity<DeliveryChallan>()
    .HasOne(d => d.Shipment)
    .WithOne(s => s.Challan)
    .HasForeignKey<Shipment>(s => s.ChallanId);

// builder.Entity<ChallanItem>().ToTable("ChallanItems", "dispatch");
// builder.Entity<ChallanItem>().Property(c => c.DispatchedQty).HasPrecision(18, 3);
// builder.Entity<ChallanItem>().Property(c => c.UnitRate).HasPrecision(18, 2);
// builder.Entity<ChallanItem>()
//     .HasOne(c => c.Challan)
//     .WithMany(c => c.Items)
//     .HasForeignKey(c => c.ChallanId)
//     .OnDelete(DeleteBehavior.Cascade);
// builder.Entity<ChallanItem>()
//     .HasOne(c => c.SOItem)
//     .WithMany()
//     .HasForeignKey(c => c.SOItemId)
//     .OnDelete(DeleteBehavior.Restrict);
builder.Entity<ChallanItem>().ToTable("ChallanItems", "dispatch");
builder.Entity<ChallanItem>().Property(c => c.DispatchedQty).HasPrecision(18, 3);
builder.Entity<ChallanItem>().Property(c => c.UnitRate).HasPrecision(18, 2);

// 1. Relationship to the Header (Challan)
builder.Entity<ChallanItem>()
    .HasOne(c => c.Challan)
    .WithMany(c => c.Items)
    .HasForeignKey(c => c.ChallanId)
    .OnDelete(DeleteBehavior.Cascade);

// 2. Relationship to the Sales Order Item
builder.Entity<ChallanItem>()
    .HasOne(c => c.SOItem)
    .WithMany()
    .HasForeignKey(c => c.SOItemId)
    .OnDelete(DeleteBehavior.Restrict);

// 3. FIX: Explicit relationship to the Item Master (The missing link)
builder.Entity<ChallanItem>()
    .HasOne(c => c.Item)
    .WithMany()
    .HasForeignKey(c => c.ItemId)
    .OnDelete(DeleteBehavior.Restrict);

builder.Entity<Shipment>().ToTable("Shipments", "dispatch");
// Sales Returns
builder.Entity<SalesReturn>().ToTable("SalesReturns", "sales");
builder.Entity<SalesReturn>().HasIndex(s => s.ReturnNo).IsUnique();
builder.Entity<SalesReturn>().Property(s => s.TotalReturnValue).HasPrecision(18, 2);
builder.Entity<SalesReturn>().HasQueryFilter(s => !s.IsDeleted);
builder.Entity<SalesReturn>()
    .HasOne(s => s.SalesOrder)
    .WithMany()
    .HasForeignKey(s => s.SOId)
    .OnDelete(DeleteBehavior.Restrict);
builder.Entity<SalesReturn>()
    .HasOne(s => s.Customer)
    .WithMany()
    .HasForeignKey(s => s.CustomerId)
    .OnDelete(DeleteBehavior.Restrict);

builder.Entity<SalesReturnItem>().ToTable("SalesReturnItems", "sales");
builder.Entity<SalesReturnItem>().Property(s => s.ReturnQty).HasPrecision(18, 3);
builder.Entity<SalesReturnItem>().Property(s => s.UnitRate).HasPrecision(18, 2);
builder.Entity<SalesReturnItem>()
    .HasOne(s => s.SalesReturn)
    .WithMany(s => s.Items)
    .HasForeignKey(s => s.SalesReturnId)
    .OnDelete(DeleteBehavior.Cascade);

// Gate Entry
builder.Entity<GateEntry>().ToTable("GateEntries", "proc");
builder.Entity<GateEntry>().HasIndex(g => g.GateEntryNo).IsUnique();
builder.Entity<GateEntry>()
    .HasOne(g => g.Vendor)
    .WithMany()
    .HasForeignKey(g => g.VendorId)
    .OnDelete(DeleteBehavior.Restrict);
    }
}
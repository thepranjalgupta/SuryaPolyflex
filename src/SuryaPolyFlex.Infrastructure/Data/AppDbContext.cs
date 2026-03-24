using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SuryaPolyFlex.Domain.Entities.Core;

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
    }
}
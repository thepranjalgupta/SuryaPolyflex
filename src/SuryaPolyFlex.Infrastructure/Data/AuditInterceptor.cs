using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SuryaPolyFlex.Domain.Entities.Core;

namespace SuryaPolyFlex.Infrastructure.Data;

public class AuditInterceptor : SaveChangesInterceptor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditInterceptor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is AppDbContext context)
            await AddAuditLogsAsync(context);

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private Task AddAuditLogsAsync(AppDbContext context)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var userId   = httpContext?.User?.FindFirst(
            System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "SYSTEM";
        var userName = httpContext?.User?.Identity?.Name ?? "SYSTEM";
        var ipAddress = httpContext?.Connection?.RemoteIpAddress?.ToString();

        var entries = context.ChangeTracker.Entries()
            .Where(e => e.Entity is not AuditLog &&
                        e.State is EntityState.Added
                               or EntityState.Modified
                               or EntityState.Deleted)
            .ToList();

        foreach (var entry in entries)
        {
            var audit = new AuditLog
            {
                TableName      = entry.Metadata.GetTableName() ?? entry.Metadata.Name,
                Action         = entry.State.ToString(),
                UserId         = userId,
                UserName       = userName,
                IpAddress      = ipAddress,
                Timestamp      = DateTime.UtcNow,
                PrimaryKey     = JsonSerializer.Serialize(
                    entry.Properties
                         .Where(p => p.Metadata.IsPrimaryKey())
                         .ToDictionary(p => p.Metadata.Name, p => p.CurrentValue)),
                AffectedColumns = entry.State == EntityState.Modified
                    ? JsonSerializer.Serialize(
                        entry.Properties
                             .Where(p => p.IsModified)
                             .Select(p => p.Metadata.Name))
                    : null,
                OldValues = entry.State is EntityState.Modified or EntityState.Deleted
                    ? JsonSerializer.Serialize(
                        entry.Properties.ToDictionary(
                            p => p.Metadata.Name, p => p.OriginalValue))
                    : null,
                NewValues = entry.State is EntityState.Added or EntityState.Modified
                    ? JsonSerializer.Serialize(
                        entry.Properties.ToDictionary(
                            p => p.Metadata.Name, p => p.CurrentValue))
                    : null
            };

            context.AuditLogs.Add(audit);
        }

        return Task.CompletedTask;
    }
}
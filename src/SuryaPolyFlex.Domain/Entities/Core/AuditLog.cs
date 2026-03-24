namespace SuryaPolyFlex.Domain.Entities.Core;

public class AuditLog
{
    public long Id { get; set; }
    public string TableName { get; set; } = default!;
    public string Action { get; set; } = default!;       // INSERT, UPDATE, DELETE
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? AffectedColumns { get; set; }
    public string? PrimaryKey { get; set; }
    public string UserId { get; set; } = default!;
    public string? UserName { get; set; }
    public string? IpAddress { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
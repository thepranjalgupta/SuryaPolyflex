namespace SuryaPolyFlex.Domain.Entities.Core;

public class Attachment
{
    public int Id { get; set; }
    public string EntityType { get; set; } = default!;   // GRN, Shipment, SalesReturn
    public int EntityId { get; set; }
    public string FileName { get; set; } = default!;
    public string FilePath { get; set; } = default!;
    public string? ContentType { get; set; }
    public long FileSizeBytes { get; set; }
    public string UploadedBy { get; set; } = default!;
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}
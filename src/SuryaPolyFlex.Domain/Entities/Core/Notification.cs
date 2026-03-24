using SuryaPolyFlex.Domain.Common;

namespace SuryaPolyFlex.Domain.Entities.Core;

public class Notification : BaseEntity
{
    public string Title { get; set; } = default!;
    public string Message { get; set; } = default!;
    public string RecipientUserId { get; set; } = default!;
    public bool IsRead { get; set; } = false;
    public DateTime? ReadAt { get; set; }
    public string? ModuleCode { get; set; }
    public int? ReferenceId { get; set; }
    public string? Url { get; set; }
}
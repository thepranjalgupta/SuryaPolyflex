using SuryaPolyFlex.Domain.Common;

namespace SuryaPolyFlex.Domain.Entities.Dispatch;

public class Transporter : BaseEntity
{
    public string Name { get; set; } = default!;
    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public string? GSTIN { get; set; }
    public bool IsActive { get; set; } = true;
}
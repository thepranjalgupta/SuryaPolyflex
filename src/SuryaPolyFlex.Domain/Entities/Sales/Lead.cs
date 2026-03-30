using SuryaPolyFlex.Domain.Common;
using SuryaPolyFlex.Domain.Enums;

namespace SuryaPolyFlex.Domain.Entities.Sales;

public class Lead : BaseEntity
{
    public string Title { get; set; } = default!;
    public int CustomerId { get; set; }
    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Source { get; set; }
    public LeadStatus Status { get; set; } = LeadStatus.New;
    public string? AssignedToId { get; set; }
    public DateTime? FollowUpDate { get; set; }
    public string? Remarks { get; set; }
    public DateTime? ConvertedAt { get; set; }

    public Customer Customer { get; set; } = default!;
}
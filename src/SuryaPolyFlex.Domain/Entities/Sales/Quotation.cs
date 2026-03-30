using SuryaPolyFlex.Domain.Common;
using SuryaPolyFlex.Domain.Enums;

namespace SuryaPolyFlex.Domain.Entities.Sales;

public class Quotation : BaseEntity
{
    public string QuotationNumber { get; set; } = default!;
    public DateTime QuotationDate { get; set; } = DateTime.UtcNow;
    public int CustomerId { get; set; }
    public int? LeadId { get; set; }
    public QuotationStatus Status { get; set; } = QuotationStatus.Draft;
    public DateTime? ValidUntil { get; set; }
    public string? Terms { get; set; }
    public string? Remarks { get; set; }
    public decimal TotalAmount { get; set; }
    public int Revision { get; set; } = 1;

    public Customer Customer { get; set; } = default!;
    public Lead? Lead { get; set; }
    public ICollection<QuotationItem> Items { get; set; } = new List<QuotationItem>();
}
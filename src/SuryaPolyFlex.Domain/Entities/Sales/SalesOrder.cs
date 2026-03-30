using SuryaPolyFlex.Domain.Common;
using SuryaPolyFlex.Domain.Enums;

namespace SuryaPolyFlex.Domain.Entities.Sales;

public class SalesOrder : BaseEntity
{
    public string SONumber { get; set; } = default!;
    public DateTime SODate { get; set; } = DateTime.UtcNow;
    public int CustomerId { get; set; }
    public int? QuotationId { get; set; }
    public SalesOrderStatus Status { get; set; } = SalesOrderStatus.Open;
    public DateTime? RequiredByDate { get; set; }
    public string? POReference { get; set; }
    public string? Remarks { get; set; }
    public decimal TotalAmount { get; set; }

    public Customer Customer { get; set; } = default!;
    public Quotation? Quotation { get; set; }
    public ICollection<SOItem> Items { get; set; } = new List<SOItem>();
    public ICollection<CustomerJob> CustomerJobs { get; set; } = new List<CustomerJob>();
    
}
using System.ComponentModel.DataAnnotations;

namespace SuryaPolyFlex.Application.Features.Quotations;

public class QuotationDto
{
    public int Id { get; set; }
    public string QuotationNumber { get; set; } = default!;
    public DateTime QuotationDate { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = default!;
    public int? LeadId { get; set; }
    public string Status { get; set; } = default!;
    public DateTime? ValidUntil { get; set; }
    public string? Terms { get; set; }
    public string? Remarks { get; set; }
    public decimal TotalAmount { get; set; }
    public int Revision { get; set; }
    public List<QuotationItemDto> Items { get; set; } = new();
}

public class QuotationItemDto
{
    public int Id { get; set; }
    public int? ItemId { get; set; }
    public string ItemCode { get; set; } = default!;
    public string Description { get; set; } = default!;
    public decimal Qty { get; set; }
    public string? UoMCode { get; set; }
    public decimal UnitRate { get; set; }
    public decimal DiscountPct { get; set; }
    public decimal TaxPct { get; set; }
    public decimal LineTotal { get; set; }
}

public class CreateQuotationDto
{
    [Required] public int CustomerId { get; set; }
    public int? LeadId { get; set; }
    public DateTime? ValidUntil { get; set; }
    public string? Terms { get; set; }
    public string? Remarks { get; set; }

    [Required] public List<CreateQuotationItemDto> Items { get; set; } = new();
}

public class CreateQuotationItemDto
{
    public int? ItemId { get; set; }
    [Required] public string Description { get; set; } = default!;
    [Required, Range(0.001, double.MaxValue)] public decimal Qty { get; set; }
    public int? UoMId { get; set; }
    [Required, Range(0.01, double.MaxValue)]  public decimal UnitRate { get; set; }
    public decimal DiscountPct { get; set; } = 0;
    public decimal TaxPct { get; set; } = 0;
}